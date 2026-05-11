'use client'
import { useState } from 'react'

const SAMPLE_TEXT = `The Australian Prudential Regulation Authority requires all regulated entities to maintain information security capability commensurate with the size and extent of threats. This includes third-party providers who process material data.

Constitutional AI is Anthropic's approach to alignment. Instead of relying solely on human feedback, the model critiques and revises its own outputs against a set of explicit principles called a constitution.

Retrieval-Augmented Generation addresses the fundamental knowledge limitations of large language models. By retrieving relevant documents at inference time, RAG systems can access current information and private data that the model was not trained on.`

function chunkText(text: string, chunkSize: number, overlap: number): string[] {
  const words = text.split(/\s+/)
  const chunks: string[] = []
  let i = 0
  while (i < words.length) {
    const chunk = words.slice(i, i + chunkSize).join(' ')
    chunks.push(chunk)
    i += chunkSize - overlap
    if (i >= words.length) break
  }
  return chunks
}

function semanticChunk(text: string): string[] {
  return text.split(/\n\n+/).map(s => s.trim()).filter(Boolean)
}

const COLORS = ['#3b82f6', '#8b5cf6', '#10b981', '#f59e0b', '#ef4444', '#06b6d4']

export default function ChunkingVisualizer() {
  const [mode, setMode] = useState<'fixed' | 'semantic'>('fixed')
  const [chunkSize, setChunkSize] = useState(30)
  const [overlap, setOverlap] = useState(5)

  const chunks = mode === 'semantic'
    ? semanticChunk(SAMPLE_TEXT)
    : chunkText(SAMPLE_TEXT, chunkSize, overlap)

  return (
    <div className="bg-gray-900 border border-gray-700 rounded-xl p-5">
      <h4 className="text-sm font-semibold text-gray-300 mb-4">Chunking Strategy Visualizer</h4>

      <div className="flex flex-wrap gap-3 mb-4">
        <div className="flex gap-2">
          {(['fixed', 'semantic'] as const).map(m => (
            <button
              key={m}
              onClick={() => setMode(m)}
              className={`text-xs px-3 py-1.5 rounded-lg border transition-colors capitalize ${
                mode === m
                  ? 'bg-blue-600 border-blue-600 text-white'
                  : 'bg-gray-800 border-gray-700 text-gray-400 hover:text-gray-200'
              }`}
            >
              {m === 'fixed' ? 'Fixed-size' : 'Semantic (paragraph)'}
            </button>
          ))}
        </div>
        {mode === 'fixed' && (
          <div className="flex items-center gap-4">
            <label className="flex items-center gap-2 text-xs text-gray-400">
              Chunk size
              <input type="range" min={15} max={60} value={chunkSize} onChange={e => setChunkSize(+e.target.value)} className="w-20" />
              <span className="text-gray-200 w-8">{chunkSize}w</span>
            </label>
            <label className="flex items-center gap-2 text-xs text-gray-400">
              Overlap
              <input type="range" min={0} max={15} value={overlap} onChange={e => setOverlap(+e.target.value)} className="w-16" />
              <span className="text-gray-200 w-6">{overlap}w</span>
            </label>
          </div>
        )}
      </div>

      {/* Chunks */}
      <div className="space-y-2 mb-4">
        {chunks.map((chunk, i) => (
          <div key={i} className="flex gap-2 items-start">
            <div
              className="text-xs font-mono px-1.5 py-0.5 rounded flex-shrink-0 mt-0.5 font-semibold"
              style={{ backgroundColor: `${COLORS[i % COLORS.length]}20`, color: COLORS[i % COLORS.length] }}
            >
              #{i + 1}
            </div>
            <div
              className="text-xs text-gray-300 leading-relaxed p-2 rounded border flex-1"
              style={{ borderColor: `${COLORS[i % COLORS.length]}30`, backgroundColor: `${COLORS[i % COLORS.length]}08` }}
            >
              {chunk}
              <span className="ml-1 text-gray-600">({chunk.split(/\s+/).length}w)</span>
            </div>
          </div>
        ))}
      </div>

      <div className="flex gap-4 text-xs text-gray-500">
        <span>{chunks.length} chunks</span>
        <span>avg {Math.round(chunks.reduce((s, c) => s + c.split(/\s+/).length, 0) / chunks.length)}w/chunk</span>
      </div>

      <div className="mt-3 grid grid-cols-2 gap-2 text-xs">
        <div className="p-2 bg-gray-800 rounded border border-gray-700">
          <div className="text-gray-300 font-medium mb-1">Fixed-size</div>
          <div className="text-gray-500">Simple, consistent size. May split mid-sentence or mid-concept.</div>
        </div>
        <div className="p-2 bg-gray-800 rounded border border-green-900/50">
          <div className="text-green-400 font-medium mb-1">Semantic</div>
          <div className="text-gray-500">Respects paragraph boundaries. Better for structured docs.</div>
        </div>
      </div>
    </div>
  )
}
