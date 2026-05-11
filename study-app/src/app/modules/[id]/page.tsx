'use client'

import { useEffect, useState } from 'react'
import { MODULES } from '@/lib/modules'
import { getQuizForModule } from '@/lib/quiz'
import { markModuleVisited, markModuleCompleted, loadProgress } from '@/lib/progress'
import RAGDiagram from '@/components/diagrams/RAGDiagram'
import CosineSimilarityDemo from '@/components/diagrams/CosineSimilarityDemo'
import AgentLoopDiagram from '@/components/diagrams/AgentLoopDiagram'
import ChunkingVisualizer from '@/components/diagrams/ChunkingVisualizer'
import SDLCDiagram from '@/components/diagrams/SDLCDiagram'

function renderBody(text: string) {
  const lines = text.split('\n')
  const elements: React.ReactNode[] = []
  let i = 0

  while (i < lines.length) {
    const line = lines[i]

    // Table detection
    if (line.includes('|') && i + 1 < lines.length && lines[i + 1].includes('---')) {
      const headers = line.split('|').map(s => s.trim()).filter(Boolean)
      i += 2 // skip separator
      const rows: string[][] = []
      while (i < lines.length && lines[i].includes('|')) {
        rows.push(lines[i].split('|').map(s => s.trim()).filter(Boolean))
        i++
      }
      elements.push(
        <table key={i} className="prose-table w-full border-collapse my-4 text-sm">
          <thead>
            <tr>{headers.map((h, j) => <th key={j} className="bg-gray-800 text-gray-400 px-3 py-2 text-left border-b border-gray-700 text-xs uppercase tracking-wider">{h}</th>)}</tr>
          </thead>
          <tbody>
            {rows.map((row, ri) => (
              <tr key={ri}>
                {row.map((cell, ci) => <td key={ci} className="px-3 py-2 border-b border-gray-800 text-gray-300 text-xs">{cell}</td>)}
              </tr>
            ))}
          </tbody>
        </table>
      )
      continue
    }

    if (line.startsWith('**') && line.endsWith('**') && !line.slice(2).includes('**')) {
      elements.push(<h4 key={i} className="text-base font-semibold text-white mt-4 mb-2">{line.slice(2, -2)}</h4>)
      i++
      continue
    }

    if (!line.trim()) { elements.push(<div key={i} className="h-2" />); i++; continue }

    // Parse inline formatting
    const parseInline = (text: string): React.ReactNode => {
      const parts = text.split(/(\*\*[^*]+\*\*|`[^`]+`)/g)
      return parts.map((part, j) => {
        if (part.startsWith('**') && part.endsWith('**')) return <strong key={j} className="text-gray-100 font-semibold">{part.slice(2, -2)}</strong>
        if (part.startsWith('`') && part.endsWith('`')) return <code key={j} className="bg-gray-800 text-cyan-300 px-1 py-0.5 rounded text-xs font-mono">{part.slice(1, -1)}</code>
        return part
      })
    }

    if (line.startsWith('- ') || line.startsWith('• ')) {
      elements.push(<li key={i} className="text-gray-300 text-sm ml-4 mb-1 list-disc">{parseInline(line.slice(2))}</li>)
    } else {
      elements.push(<p key={i} className="text-gray-300 text-sm leading-relaxed mb-2">{parseInline(line)}</p>)
    }
    i++
  }

  return elements
}

export default function ModulePage({ params }: { params: { id: string } }) {
  const mod = MODULES.find(m => m.id === params.id)
  const quiz = getQuizForModule(params.id)
  const [completed, setCompleted] = useState(false)

  useEffect(() => {
    if (!mod) return
    markModuleVisited(mod.id)
    const p = loadProgress()
    setCompleted(p.modules[mod.id]?.completed ?? false)
  }, [mod])

  if (!mod) return (
    <div className="max-w-4xl mx-auto px-4 py-16 text-center">
      <p className="text-gray-400">Module not found.</p>
      <a href="/" className="text-blue-400 hover:underline mt-4 inline-block">← Back to Dashboard</a>
    </div>
  )

  const modIndex = MODULES.findIndex(m => m.id === params.id)
  const prev = modIndex > 0 ? MODULES[modIndex - 1] : null
  const next = modIndex < MODULES.length - 1 ? MODULES[modIndex + 1] : null

  const handleComplete = () => {
    markModuleCompleted(mod.id)
    setCompleted(true)
  }

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Breadcrumb */}
      <div className="flex items-center gap-2 text-sm text-gray-500 mb-6">
        <a href="/" className="hover:text-gray-300 transition-colors">Dashboard</a>
        <span>›</span>
        <span className={mod.color}>{mod.title}</span>
      </div>

      {/* Header */}
      <div className={`${mod.bgColor} border ${mod.borderColor} rounded-2xl p-6 mb-8`}>
        <div className="flex items-start justify-between">
          <div>
            <span className="text-4xl mb-3 block">{mod.icon}</span>
            <h1 className="text-2xl font-bold text-white mb-2">{mod.title}</h1>
            <p className="text-gray-400 text-sm">{mod.subtitle}</p>
          </div>
          <div className="text-right flex flex-col items-end gap-2">
            <span className="text-xs text-gray-500">~{mod.estimatedMins} min</span>
            {completed && <span className="text-xs bg-green-900/60 text-green-400 border border-green-700/50 px-2 py-0.5 rounded-full">✓ Completed</span>}
          </div>
        </div>
      </div>

      {/* Sections */}
      <div className="space-y-8">
        {mod.sections.map((section, si) => (
          <div key={si} className="bg-gray-900 border border-gray-800 rounded-xl overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-800">
              <h2 className="text-lg font-semibold text-white">{section.title}</h2>
            </div>
            <div className="p-6">
              <div className="prose-dark">{renderBody(section.body)}</div>

              {/* Interactive diagram */}
              {section.diagram === 'rag' && <div className="mt-6"><RAGDiagram /></div>}
              {section.diagram === 'cosine' && <div className="mt-6"><CosineSimilarityDemo /></div>}
              {section.diagram === 'agent' && <div className="mt-6"><AgentLoopDiagram /></div>}
              {section.diagram === 'chunking' && <div className="mt-6"><ChunkingVisualizer /></div>}
              {section.diagram === 'sdlc' && <div className="mt-6"><SDLCDiagram /></div>}

              {/* Code block */}
              {section.code && (
                <div className="mt-6">
                  <pre className="bg-gray-950 border border-gray-700 rounded-xl p-4 overflow-x-auto">
                    <code className="text-sm text-green-300 font-mono">{section.code}</code>
                  </pre>
                </div>
              )}

              {/* Key points */}
              {section.keyPoints && (
                <div className="mt-6 bg-gray-800/50 border border-gray-700 rounded-xl p-4">
                  <h4 className="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-3">Key Points</h4>
                  <ul className="space-y-2">
                    {section.keyPoints.map((pt, pi) => (
                      <li key={pi} className="flex items-start gap-2 text-sm text-gray-300">
                        <span className={`${mod.color} mt-0.5 flex-shrink-0`}>›</span>
                        {pt}
                      </li>
                    ))}
                  </ul>
                </div>
              )}
            </div>
          </div>
        ))}
      </div>

      {/* Actions */}
      <div className="mt-8 flex flex-col sm:flex-row gap-3">
        {!completed && (
          <button
            onClick={handleComplete}
            className="flex-1 py-3 bg-green-700 hover:bg-green-600 text-white rounded-xl font-medium transition-colors text-sm"
          >
            ✓ Mark as Complete
          </button>
        )}
        {quiz && (
          <a
            href={`/quiz/${mod.id}`}
            className={`${completed ? 'flex-1' : ''} py-3 px-6 bg-blue-700 hover:bg-blue-600 text-white rounded-xl font-medium transition-colors text-sm text-center`}
          >
            Take Quiz →
          </a>
        )}
      </div>

      {/* Navigation */}
      <div className="mt-8 flex justify-between">
        {prev ? (
          <a href={`/modules/${prev.id}`} className="flex items-center gap-2 text-sm text-gray-400 hover:text-gray-200 transition-colors">
            <span>←</span>
            <div>
              <div className="text-xs text-gray-600">Previous</div>
              <div>{prev.title}</div>
            </div>
          </a>
        ) : <div />}
        {next ? (
          <a href={`/modules/${next.id}`} className="flex items-center gap-2 text-sm text-gray-400 hover:text-gray-200 transition-colors">
            <div className="text-right">
              <div className="text-xs text-gray-600">Next</div>
              <div>{next.title}</div>
            </div>
            <span>→</span>
          </a>
        ) : <div />}
      </div>
    </div>
  )
}
