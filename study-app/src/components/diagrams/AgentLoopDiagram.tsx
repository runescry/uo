'use client'
import { useState } from 'react'

const SCENARIO = [
  { phase: 'Think', icon: '🧠', color: '#3b82f6', label: 'Reason', text: 'I need to find the CPS 234 requirements. I should search the document database.' },
  { phase: 'Act', icon: '🔧', color: '#f59e0b', label: 'Call Tool', text: 'search_documents({ query: "APRA CPS 234 AI requirements", max_results: 5 })' },
  { phase: 'Observe', icon: '👁️', color: '#10b981', label: 'Get Result', text: 'Returned 3 documents: "CPS234_policy.pdf", "AI_governance_framework.docx", "Third_party_risk.pdf"' },
  { phase: 'Think', icon: '🧠', color: '#3b82f6', label: 'Reason', text: 'I have the relevant documents. I can now summarize the key CPS 234 requirements for AI.' },
  { phase: 'Act', icon: '🔧', color: '#f59e0b', label: 'Call Tool', text: 'read_document({ id: "CPS234_policy.pdf", section: "AI and Machine Learning" })' },
  { phase: 'Observe', icon: '👁️', color: '#10b981', label: 'Get Result', text: 'Retrieved content: "Third parties processing material data must be assessed for security capability..."' },
  { phase: 'Respond', icon: '✅', color: '#8b5cf6', label: 'Answer', text: 'CPS 234 requires: (1) Third-party risk assessment for AI providers, (2) Data classification alignment, (3) 72-hour incident notification...' },
]

export default function AgentLoopDiagram() {
  const [step, setStep] = useState(-1)

  return (
    <div className="bg-gray-900 border border-gray-700 rounded-xl p-5">
      <div className="flex items-center justify-between mb-4">
        <div>
          <h4 className="text-sm font-semibold text-gray-300">ReAct Agent Loop</h4>
          <p className="text-xs text-gray-500">Think → Act → Observe → Think → Act → Observe → Respond</p>
        </div>
        <div className="flex gap-2">
          <button
            onClick={() => setStep(Math.min(step + 1, SCENARIO.length - 1))}
            disabled={step >= SCENARIO.length - 1}
            className="text-xs px-3 py-1.5 bg-blue-600 hover:bg-blue-500 disabled:bg-gray-700 disabled:text-gray-500 text-white rounded-lg transition-colors"
          >
            Next Step
          </button>
          <button
            onClick={() => setStep(-1)}
            className="text-xs px-3 py-1.5 bg-gray-700 hover:bg-gray-600 text-gray-300 rounded-lg transition-colors"
          >
            Reset
          </button>
        </div>
      </div>

      {/* Loop visualization */}
      <div className="flex items-center gap-1 mb-4 flex-wrap">
        {['Think', 'Act', 'Observe'].map((phase, i) => (
          <div key={i} className="flex items-center gap-1">
            <div className="text-xs px-2 py-1 rounded-md font-medium" style={{
              backgroundColor: phase === 'Think' ? '#1d40af40' : phase === 'Act' ? '#92400e40' : '#065f4640',
              color: phase === 'Think' ? '#93c5fd' : phase === 'Act' ? '#fcd34d' : '#6ee7b7',
              border: `1px solid ${phase === 'Think' ? '#1d4ed840' : phase === 'Act' ? '#d9770640' : '#05966940'}`,
            }}>{phase}</div>
            {i < 2 && <span className="text-gray-600 text-xs">→</span>}
          </div>
        ))}
        <span className="text-gray-600 text-xs">→ ... → </span>
        <div className="text-xs px-2 py-1 rounded-md font-medium bg-purple-900/40 text-purple-300 border border-purple-700/40">Respond</div>
      </div>

      {/* Steps */}
      <div className="space-y-2">
        {SCENARIO.map((s, i) => (
          <div
            key={i}
            className={`flex gap-3 p-3 rounded-lg border transition-all duration-300 ${
              i <= step
                ? 'opacity-100'
                : 'opacity-20'
            } ${i === step ? 'scale-[1.01]' : ''}`}
            style={{
              borderColor: i <= step ? `${s.color}40` : '#1f2937',
              backgroundColor: i === step ? `${s.color}10` : '#111827',
            }}
          >
            <div className="flex flex-col items-center gap-1 flex-shrink-0">
              <span className="text-lg">{s.icon}</span>
              <span className="text-xs font-medium" style={{ color: s.color }}>{s.label}</span>
            </div>
            <div className="flex-1 min-w-0">
              <code className="text-xs text-gray-300 font-mono leading-relaxed break-words">{s.text}</code>
            </div>
          </div>
        ))}
      </div>

      {step === -1 && (
        <p className="text-center text-xs text-gray-600 mt-3">Click "Next Step" to trace an agent researching CPS 234</p>
      )}

      {/* Key insight */}
      <div className="mt-4 p-3 bg-amber-950/30 border border-amber-800/40 rounded-lg">
        <p className="text-xs text-amber-300/80">
          <strong className="text-amber-300">Key insight:</strong> Claude doesn't execute tools — it generates a structured request. Your application code executes the tool and returns results. This boundary is where safety controls live.
        </p>
      </div>
    </div>
  )
}
