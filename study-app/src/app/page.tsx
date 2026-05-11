'use client'

import { useEffect, useState } from 'react'
import { MODULES } from '@/lib/modules'
import { QUIZZES } from '@/lib/quiz'
import { FLASHCARDS } from '@/lib/flashcards'
import { loadProgress, getOverallStats, StudyProgress } from '@/lib/progress'

const CHECKLIST = [
  'Explain the transformer architecture in 90 seconds to a non-technical executive',
  'Explain RAG, why it exists, and its two main failure modes',
  'Explain Constitutional AI vs RLHF clearly',
  'Name Claude\'s model family and the right tier for 3 different use cases',
  'Explain what MCP is and give one enterprise use case',
  'Describe APRA CPS 234\'s key requirements for AI deployments',
  'Articulate Claude\'s differentiation vs GPT-4o in 60 seconds',
  'Walk through the LLM NPC architecture (Problem → Architecture → Result)',
  'Name three agent failure modes and their mitigations',
  'Explain when to use fine-tuning vs. prompt engineering',
]

export default function Dashboard() {
  const [progress, setProgress] = useState<StudyProgress>({ modules: {}, flashcardsReviewed: [], interviewQuestionsReviewed: [] })
  const [checklist, setChecklist] = useState<boolean[]>(new Array(CHECKLIST.length).fill(false))

  useEffect(() => {
    setProgress(loadProgress())
    try {
      const saved = localStorage.getItem('pre-screener-checklist')
      if (saved) setChecklist(JSON.parse(saved))
    } catch { /* ignore */ }
  }, [])

  const toggleCheck = (i: number) => {
    const next = [...checklist]
    next[i] = !next[i]
    setChecklist(next)
    localStorage.setItem('pre-screener-checklist', JSON.stringify(next))
  }

  const stats = getOverallStats(progress, MODULES.length, FLASHCARDS.length)
  const modulesWithQuiz = new Set(QUIZZES.map(q => q.moduleId))

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-white mb-2">Anthropic Interview Prep</h1>
        <p className="text-gray-400">Head of ANZ, Applied AI — Technical Study System</p>
      </div>

      {/* Stats bar */}
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-10">
        {[
          { label: 'Modules Read', value: `${stats.modulesCompleted}/${stats.totalModules}`, color: 'text-blue-400' },
          { label: 'Quizzes Taken', value: `${stats.quizzesTaken}/${QUIZZES.length}`, color: 'text-green-400' },
          { label: 'Avg Quiz Score', value: stats.quizzesTaken > 0 ? `${stats.avgQuizScore}%` : '—', color: 'text-yellow-400' },
          { label: 'Flashcards', value: `${stats.flashcardsReviewed}/${stats.totalCards}`, color: 'text-purple-400' },
        ].map(s => (
          <div key={s.label} className="bg-gray-900 border border-gray-800 rounded-xl p-4">
            <div className={`text-2xl font-bold ${s.color}`}>{s.value}</div>
            <div className="text-xs text-gray-500 mt-1">{s.label}</div>
          </div>
        ))}
      </div>

      {/* Progress bar */}
      <div className="mb-10">
        <div className="flex justify-between text-sm text-gray-400 mb-2">
          <span>Overall progress</span>
          <span>{stats.completionPercent}%</span>
        </div>
        <div className="h-2 bg-gray-800 rounded-full overflow-hidden">
          <div
            className="h-full bg-gradient-to-r from-blue-500 to-violet-500 rounded-full transition-all duration-700"
            style={{ width: `${stats.completionPercent}%` }}
          />
        </div>
      </div>

      {/* Modules grid */}
      <h2 className="text-xl font-semibold text-white mb-4">Study Modules</h2>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 mb-12">
        {MODULES.map(mod => {
          const mp = progress.modules[mod.id]
          const hasQuiz = modulesWithQuiz.has(mod.id)
          return (
            <a
              key={mod.id}
              href={`/modules/${mod.id}`}
              className={`${mod.bgColor} border ${mod.borderColor} rounded-xl p-5 hover:scale-[1.02] transition-all duration-200 group block`}
            >
              <div className="flex items-start justify-between mb-3">
                <span className="text-2xl">{mod.icon}</span>
                <div className="flex items-center gap-2">
                  {mp?.completed && (
                    <span className="text-xs bg-green-900/60 text-green-400 border border-green-700/50 px-2 py-0.5 rounded-full">✓ Read</span>
                  )}
                  {mp?.quizAttempted && (
                    <span className="text-xs bg-yellow-900/60 text-yellow-400 border border-yellow-700/50 px-2 py-0.5 rounded-full">{mp.quizScore}%</span>
                  )}
                </div>
              </div>
              <h3 className={`font-semibold text-white text-sm mb-1 group-hover:${mod.color} transition-colors`}>{mod.title}</h3>
              <p className="text-xs text-gray-400 mb-3 leading-relaxed">{mod.subtitle}</p>
              <div className="flex items-center justify-between">
                <span className="text-xs text-gray-500">~{mod.estimatedMins} min</span>
                {hasQuiz && <span className="text-xs text-gray-500">Quiz available</span>}
              </div>
            </a>
          )
        })}
      </div>

      {/* Pre-screener checklist */}
      <div className="bg-gray-900 border border-gray-800 rounded-xl p-6 mb-10">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-lg font-semibold text-white">Pre-Screener Checklist</h2>
          <span className="text-sm text-gray-400">{checklist.filter(Boolean).length}/{CHECKLIST.length} ready</span>
        </div>
        <p className="text-sm text-gray-400 mb-4">Can you do each of these without notes?</p>
        <div className="space-y-2">
          {CHECKLIST.map((item, i) => (
            <label key={i} className="flex items-start gap-3 cursor-pointer group">
              <div
                className={`w-5 h-5 mt-0.5 rounded border-2 flex items-center justify-center flex-shrink-0 transition-colors ${
                  checklist[i] ? 'bg-green-600 border-green-600' : 'border-gray-600 group-hover:border-gray-400'
                }`}
                onClick={() => toggleCheck(i)}
              >
                {checklist[i] && <span className="text-white text-xs">✓</span>}
              </div>
              <span className={`text-sm transition-colors ${checklist[i] ? 'text-gray-500 line-through' : 'text-gray-300'}`}>
                {item}
              </span>
            </label>
          ))}
        </div>
      </div>

      {/* Quick links */}
      <div className="grid grid-cols-2 gap-4">
        <a href="/flashcards" className="bg-purple-950/40 border border-purple-700/50 rounded-xl p-5 hover:scale-[1.02] transition-all text-center">
          <div className="text-3xl mb-2">🃏</div>
          <div className="font-semibold text-white text-sm mb-1">Flashcards</div>
          <div className="text-xs text-gray-400">{FLASHCARDS.length} terms across all modules</div>
        </a>
        <a href="/interview" className="bg-emerald-950/40 border border-emerald-700/50 rounded-xl p-5 hover:scale-[1.02] transition-all text-center">
          <div className="text-3xl mb-2">🎯</div>
          <div className="font-semibold text-white text-sm mb-1">Interview Practice</div>
          <div className="text-xs text-gray-400">Q&A with reveal — practice out loud</div>
        </a>
      </div>
    </div>
  )
}
