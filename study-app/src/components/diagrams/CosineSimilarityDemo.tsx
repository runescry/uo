'use client'
import { useState } from 'react'

// Simulated cosine similarity using keyword overlap + semantic rules
function simulateSimilarity(a: string, b: string): number {
  const normalize = (s: string) => s.toLowerCase().replace(/[^a-z0-9 ]/g, '').split(' ').filter(Boolean)
  const wordsA = new Set(normalize(a))
  const wordsB = new Set(normalize(b))

  const synonymGroups = [
    ['llm', 'language model', 'ai model', 'gpt', 'claude', 'model'],
    ['rag', 'retrieval', 'search', 'retrieve', 'lookup'],
    ['embedding', 'vector', 'encode', 'representation'],
    ['agent', 'autonomous', 'agentic'],
    ['chunk', 'split', 'segment', 'piece'],
    ['database', 'db', 'store', 'storage'],
    ['training', 'fine-tune', 'finetune', 'pretrain'],
    ['australia', 'australian', 'anz', 'apra'],
    ['security', 'safe', 'safety', 'secure'],
  ]

  let sharedTerms = 0
  const totalTerms = Math.max(wordsA.size + wordsB.size, 1)

  // Direct word overlap
  for (const w of wordsA) {
    if (wordsB.has(w) && w.length > 2) sharedTerms += 2
  }

  // Synonym group overlap
  for (const group of synonymGroups) {
    const aHas = group.some(t => [...wordsA].some(w => t.includes(w) || w.includes(t)))
    const bHas = group.some(t => [...wordsB].some(w => t.includes(w) || w.includes(t)))
    if (aHas && bHas) sharedTerms += 3
  }

  const raw = Math.min(sharedTerms / totalTerms * 2.5, 1)

  // Add noise for realism
  const noise = (Math.random() - 0.5) * 0.05
  return Math.max(0, Math.min(1, raw + noise))
}

function getSimilarityLabel(score: number): { label: string; color: string; desc: string } {
  if (score > 0.85) return { label: 'Very Similar', color: '#10b981', desc: 'These texts have nearly the same meaning' }
  if (score > 0.65) return { label: 'Related', color: '#f59e0b', desc: 'These texts share significant semantic overlap' }
  if (score > 0.4) return { label: 'Somewhat Related', color: '#f97316', desc: 'Some shared concepts, different focus' }
  return { label: 'Unrelated', color: '#ef4444', desc: 'These texts have very different meanings' }
}

const EXAMPLES = [
  { a: 'RAG retrieves documents to augment LLM responses', b: 'Retrieval augmented generation fetches relevant content for the model', label: 'High similarity (synonyms)' },
  { a: 'Claude uses Constitutional AI for safety alignment', b: 'RLHF trains models using human preference rankings', label: 'Medium similarity (same domain)' },
  { a: 'The weather in Melbourne is mild in spring', b: 'APRA CPS 234 requires third-party security assessments', label: 'Low similarity (unrelated)' },
]

export default function CosineSimilarityDemo() {
  const [textA, setTextA] = useState('RAG retrieves documents to augment LLM responses')
  const [textB, setTextB] = useState('Retrieval augmented generation fetches relevant content for the model')
  const [score, setScore] = useState<number | null>(null)

  const compute = () => {
    if (!textA.trim() || !textB.trim()) return
    setScore(simulateSimilarity(textA, textB))
  }

  const loadExample = (ex: typeof EXAMPLES[0]) => {
    setTextA(ex.a)
    setTextB(ex.b)
    setScore(simulateSimilarity(ex.a, ex.b))
  }

  const info = score !== null ? getSimilarityLabel(score) : null

  return (
    <div className="bg-gray-900 border border-gray-700 rounded-xl p-5">
      <h4 className="text-sm font-semibold text-gray-300 mb-1">Cosine Similarity Explorer</h4>
      <p className="text-xs text-gray-500 mb-4">Approximates semantic similarity between two texts (simulated — no API call)</p>

      {/* Example buttons */}
      <div className="flex flex-wrap gap-2 mb-4">
        {EXAMPLES.map((ex, i) => (
          <button
            key={i}
            onClick={() => loadExample(ex)}
            className="text-xs px-2.5 py-1 bg-gray-800 hover:bg-gray-700 border border-gray-700 rounded-lg text-gray-300 transition-colors"
          >
            {ex.label}
          </button>
        ))}
      </div>

      <div className="grid grid-cols-2 gap-3 mb-4">
        <div>
          <label className="text-xs text-gray-400 mb-1.5 block">Text A</label>
          <textarea
            value={textA}
            onChange={e => setTextA(e.target.value)}
            rows={3}
            className="w-full bg-gray-800 border border-gray-700 rounded-lg px-3 py-2 text-sm text-gray-200 resize-none focus:outline-none focus:border-blue-500"
          />
        </div>
        <div>
          <label className="text-xs text-gray-400 mb-1.5 block">Text B</label>
          <textarea
            value={textB}
            onChange={e => setTextB(e.target.value)}
            rows={3}
            className="w-full bg-gray-800 border border-gray-700 rounded-lg px-3 py-2 text-sm text-gray-200 resize-none focus:outline-none focus:border-blue-500"
          />
        </div>
      </div>

      <button
        onClick={compute}
        className="w-full py-2 bg-blue-600 hover:bg-blue-500 text-white text-sm rounded-lg transition-colors mb-4"
      >
        Compute Similarity
      </button>

      {score !== null && info && (
        <div className="bg-gray-800 rounded-lg p-4 animate-fade-in">
          <div className="flex items-center justify-between mb-3">
            <span className="text-sm font-medium" style={{ color: info.color }}>{info.label}</span>
            <span className="text-2xl font-bold" style={{ color: info.color }}>{score.toFixed(3)}</span>
          </div>

          {/* Score bar */}
          <div className="h-3 bg-gray-700 rounded-full overflow-hidden mb-3">
            <div
              className="h-full rounded-full transition-all duration-700"
              style={{ width: `${score * 100}%`, backgroundColor: info.color }}
            />
          </div>

          {/* Scale markers */}
          <div className="flex justify-between text-xs text-gray-600 mb-3">
            <span>0.0 (Opposite)</span>
            <span>0.5 (Unrelated)</span>
            <span>1.0 (Identical)</span>
          </div>

          <p className="text-xs text-gray-400">{info.desc}</p>

          <div className="mt-3 pt-3 border-t border-gray-700 text-xs text-gray-500">
            <strong className="text-gray-400">In production RAG:</strong> Threshold 0.70+ = "relevant chunk to retrieve."
            Threshold 0.85+ = "nearly duplicate — use for semantic caching."
          </div>
        </div>
      )}
    </div>
  )
}
