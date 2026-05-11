'use client'
import { useState, useEffect } from 'react'

const STEPS = [
  { id: 0, label: 'User Query', desc: 'Player asks NPC about the Crimson Sanctum', icon: '💬', color: '#3b82f6' },
  { id: 1, label: 'Embed Query', desc: 'Convert query to a 768-dim vector using nomic-embed-text', icon: '🔢', color: '#8b5cf6' },
  { id: 2, label: 'Vector Search', desc: 'Find top-k chunks by cosine similarity in vector store', icon: '🔍', color: '#06b6d4' },
  { id: 3, label: 'Rerank', desc: 'Cross-encoder re-scores candidates for precision', icon: '📊', color: '#f59e0b' },
  { id: 4, label: 'Inject Context', desc: 'Retrieved lore injected into system prompt', icon: '📄', color: '#10b981' },
  { id: 5, label: 'Generate', desc: 'Claude generates a grounded, lore-accurate response', icon: '✨', color: '#f43f5e' },
]

export default function RAGDiagram() {
  const [active, setActive] = useState(-1)
  const [running, setRunning] = useState(false)

  const runAnimation = () => {
    setRunning(true)
    setActive(-1)
    let i = 0
    const tick = () => {
      setActive(i)
      i++
      if (i < STEPS.length) setTimeout(tick, 900)
      else setTimeout(() => { setRunning(false) }, 900)
    }
    tick()
  }

  return (
    <div className="bg-gray-900 border border-gray-700 rounded-xl p-5">
      <div className="flex items-center justify-between mb-4">
        <h4 className="text-sm font-semibold text-gray-300">RAG Pipeline — Animated</h4>
        <button
          onClick={runAnimation}
          disabled={running}
          className="text-xs px-3 py-1.5 bg-blue-600 hover:bg-blue-500 disabled:bg-gray-700 disabled:text-gray-500 text-white rounded-lg transition-colors"
        >
          {running ? 'Running...' : '▶ Run Pipeline'}
        </button>
      </div>

      {/* Flow */}
      <div className="flex flex-wrap gap-2 items-center justify-center mb-4">
        {STEPS.map((step, i) => (
          <div key={step.id} className="flex items-center gap-2">
            <div
              className={`relative flex flex-col items-center p-3 rounded-lg border transition-all duration-300 min-w-[90px] text-center ${
                active === i
                  ? 'border-opacity-100 scale-110 shadow-lg'
                  : active > i
                  ? 'opacity-70 scale-100'
                  : 'opacity-40 scale-100'
              }`}
              style={{
                borderColor: active >= i ? step.color : '#374151',
                backgroundColor: active === i ? `${step.color}20` : '#111827',
                boxShadow: active === i ? `0 0 16px ${step.color}60` : 'none',
              }}
            >
              <span className="text-lg mb-1">{step.icon}</span>
              <span className="text-xs font-medium text-gray-200">{step.label}</span>
            </div>
            {i < STEPS.length - 1 && (
              <div className={`text-gray-600 text-sm transition-colors duration-300 ${active > i ? 'text-gray-400' : ''}`}>→</div>
            )}
          </div>
        ))}
      </div>

      {/* Active step description */}
      <div className="h-10 flex items-center justify-center">
        {active >= 0 && active < STEPS.length && (
          <p className="text-sm text-gray-400 text-center animate-fade-in">
            <span style={{ color: STEPS[active].color }} className="font-medium">{STEPS[active].label}: </span>
            {STEPS[active].desc}
          </p>
        )}
        {active === -1 && <p className="text-xs text-gray-600">Click "Run Pipeline" to see how a query flows through RAG</p>}
      </div>

      {/* Proactive vs Reactive comparison */}
      <div className="mt-4 grid grid-cols-2 gap-3 text-xs">
        <div className="bg-gray-800 rounded-lg p-3 border border-gray-700">
          <div className="text-red-400 font-medium mb-1">❌ Reactive RAG</div>
          <div className="text-gray-400">Each query triggers embed + search<br/>+200-500ms latency per turn</div>
        </div>
        <div className="bg-gray-800 rounded-lg p-3 border border-green-800">
          <div className="text-green-400 font-medium mb-1">✅ Proactive RAG (NPC System)</div>
          <div className="text-gray-400">Context pre-loaded at spawn<br/>Zero per-query latency</div>
        </div>
      </div>
    </div>
  )
}
