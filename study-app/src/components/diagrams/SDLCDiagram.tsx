'use client'
import { useState } from 'react'

const PHASES = [
  {
    phase: 'Plan', icon: '📋',
    traditional: 'Manual spec writing, requirements gathering meetings',
    withAI: 'AI decomposes requirements, flags ambiguities in PRDs, generates technical specs',
    tool: 'Claude + Claude Code',
    impact: 'High',
  },
  {
    phase: 'Code', icon: '💻',
    traditional: 'Engineers write all code from scratch or with autocomplete',
    withAI: 'Claude Code reads codebase, writes consistent code, generates boilerplate, implements features from specs',
    tool: 'Claude Code',
    impact: 'Very High',
  },
  {
    phase: 'Review', icon: '🔍',
    traditional: 'Manual peer review — often rushed, inconsistent coverage',
    withAI: 'AI reviews every diff: bugs, security (OWASP), performance, style. PR descriptions auto-generated.',
    tool: 'Claude Code + API',
    impact: 'High',
  },
  {
    phase: 'Test', icon: '🧪',
    traditional: 'Developers write tests manually — often incomplete coverage',
    withAI: 'Given implementation, generate unit + integration tests covering happy paths and edge cases',
    tool: 'Claude Code',
    impact: 'Very High',
  },
  {
    phase: 'Deploy', icon: '🚀',
    traditional: 'CI/CD runs fixed pipeline, manual runbooks for incidents',
    withAI: 'AI-assisted incident diagnosis, log analysis, rollback recommendation',
    tool: 'Claude API',
    impact: 'Medium',
  },
  {
    phase: 'Document', icon: '📝',
    traditional: 'Often skipped or done poorly — low priority, time-consuming',
    withAI: 'Auto-generate docstrings, README, API docs, ADRs from code',
    tool: 'Claude API',
    impact: 'Medium',
  },
]

export default function SDLCDiagram() {
  const [selected, setSelected] = useState<number | null>(null)

  return (
    <div className="bg-gray-900 border border-gray-700 rounded-xl p-5">
      <h4 className="text-sm font-semibold text-gray-300 mb-4">AI-Assisted SDLC — Click a Phase</h4>

      {/* Phase pills */}
      <div className="flex flex-wrap gap-2 mb-5">
        {PHASES.map((p, i) => (
          <button
            key={i}
            onClick={() => setSelected(selected === i ? null : i)}
            className={`flex items-center gap-1.5 text-sm px-3 py-1.5 rounded-lg border transition-all ${
              selected === i
                ? 'bg-indigo-600 border-indigo-500 text-white'
                : 'bg-gray-800 border-gray-700 text-gray-400 hover:text-gray-200 hover:border-gray-600'
            }`}
          >
            <span>{p.icon}</span>
            <span>{p.phase}</span>
          </button>
        ))}
      </div>

      {/* Selected phase detail */}
      {selected !== null && (
        <div className="bg-gray-800 rounded-xl p-4 border border-indigo-700/40 animate-fade-in">
          <div className="flex items-center justify-between mb-3">
            <h5 className="font-semibold text-white flex items-center gap-2">
              <span>{PHASES[selected].icon}</span>
              {PHASES[selected].phase} Phase
            </h5>
            <span className={`text-xs px-2 py-0.5 rounded-full ${
              PHASES[selected].impact === 'Very High' ? 'bg-green-900/60 text-green-400 border border-green-700/50' :
              PHASES[selected].impact === 'High' ? 'bg-blue-900/60 text-blue-400 border border-blue-700/50' :
              'bg-gray-700 text-gray-400 border border-gray-600'
            }`}>
              AI Impact: {PHASES[selected].impact}
            </span>
          </div>

          <div className="grid grid-cols-2 gap-3 text-xs mb-3">
            <div className="bg-gray-900 rounded-lg p-3 border border-gray-700">
              <div className="text-gray-500 mb-1 font-medium">Without AI</div>
              <div className="text-gray-300">{PHASES[selected].traditional}</div>
            </div>
            <div className="bg-indigo-950/40 rounded-lg p-3 border border-indigo-700/40">
              <div className="text-indigo-400 mb-1 font-medium">With AI (Claude)</div>
              <div className="text-gray-300">{PHASES[selected].withAI}</div>
            </div>
          </div>

          <div className="text-xs text-gray-500">
            <span className="text-gray-400">Tool: </span>{PHASES[selected].tool}
          </div>
        </div>
      )}

      {selected === null && (
        <div className="text-center text-xs text-gray-600 py-4">Select a phase to see how AI transforms it</div>
      )}

      <div className="mt-4 p-3 bg-gray-800 rounded-lg border border-gray-700 text-xs text-gray-400">
        <strong className="text-gray-300">The strategic shift:</strong> AI doesn't just make coders faster — it changes who can build. The bottleneck moves from <em>implementation</em> to <em>knowing what to build</em>. Product managers, analysts, and domain experts can now prototype working software.
      </div>
    </div>
  )
}
