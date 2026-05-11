'use client'

import { useState, useEffect } from 'react'
import { MODULES } from '@/lib/modules'
import { getQuizForModule } from '@/lib/quiz'
import { saveQuizScore } from '@/lib/progress'

export default function QuizPage({ params }: { params: { id: string } }) {
  const mod = MODULES.find(m => m.id === params.id)
  const quiz = getQuizForModule(params.id)
  const [answers, setAnswers] = useState<Record<string, number>>({})
  const [submitted, setSubmitted] = useState(false)
  const [revealed, setRevealed] = useState<Record<string, boolean>>({})

  if (!mod || !quiz) return (
    <div className="max-w-2xl mx-auto px-4 py-16 text-center">
      <p className="text-gray-400">No quiz found.</p>
      <a href="/" className="text-blue-400 hover:underline mt-4 inline-block">← Dashboard</a>
    </div>
  )

  const score = submitted
    ? quiz.questions.filter((q, i) => answers[q.id] === q.correctIndex).length
    : 0
  const pct = Math.round((score / quiz.questions.length) * 100)

  const handleSubmit = () => {
    setSubmitted(true)
    saveQuizScore(mod.id, pct)
    setRevealed(Object.fromEntries(quiz.questions.map(q => [q.id, true])))
  }

  const handleReset = () => {
    setAnswers({})
    setSubmitted(false)
    setRevealed({})
  }

  const allAnswered = quiz.questions.every(q => answers[q.id] !== undefined)

  return (
    <div className="max-w-3xl mx-auto px-4 sm:px-6 py-8">
      {/* Header */}
      <div className="flex items-center gap-3 mb-6">
        <a href={`/modules/${mod.id}`} className="text-gray-500 hover:text-gray-300 transition-colors text-sm">← {mod.title}</a>
      </div>

      <div className={`${mod.bgColor} border ${mod.borderColor} rounded-2xl p-6 mb-8`}>
        <div className="flex items-center gap-3">
          <span className="text-3xl">{mod.icon}</span>
          <div>
            <h1 className="text-xl font-bold text-white">{mod.title} — Quiz</h1>
            <p className="text-sm text-gray-400">{quiz.questions.length} questions</p>
          </div>
        </div>
      </div>

      {/* Score banner */}
      {submitted && (
        <div className={`rounded-xl p-5 mb-8 border ${
          pct >= 80 ? 'bg-green-950/50 border-green-700/50' :
          pct >= 60 ? 'bg-yellow-950/50 border-yellow-700/50' :
          'bg-red-950/50 border-red-700/50'
        } animate-slide-up`}>
          <div className="flex items-center justify-between">
            <div>
              <div className={`text-4xl font-bold mb-1 ${pct >= 80 ? 'text-green-400' : pct >= 60 ? 'text-yellow-400' : 'text-red-400'}`}>
                {pct}%
              </div>
              <div className="text-gray-400 text-sm">{score} / {quiz.questions.length} correct</div>
              <div className="text-gray-400 text-sm mt-1">
                {pct >= 80 ? '✓ Strong — review any wrong answers then move on' :
                 pct >= 60 ? '→ Review the explanations for incorrect answers, re-take' :
                 '✗ Re-read the module, then try again'}
              </div>
            </div>
            <button onClick={handleReset} className="text-sm px-4 py-2 bg-gray-700 hover:bg-gray-600 text-gray-200 rounded-lg transition-colors">
              Try Again
            </button>
          </div>
        </div>
      )}

      {/* Questions */}
      <div className="space-y-6">
        {quiz.questions.map((q, qi) => {
          const selected = answers[q.id]
          const isCorrect = selected === q.correctIndex
          const show = revealed[q.id]

          return (
            <div key={q.id} className={`bg-gray-900 border rounded-xl overflow-hidden transition-all ${
              show
                ? isCorrect ? 'border-green-700/60' : 'border-red-700/60'
                : 'border-gray-800'
            }`}>
              <div className="px-6 py-4 border-b border-gray-800">
                <div className="flex items-start gap-3">
                  <span className="text-xs text-gray-500 bg-gray-800 px-2 py-1 rounded-full flex-shrink-0 mt-0.5">Q{qi + 1}</span>
                  <p className="text-sm text-gray-100 font-medium leading-relaxed">{q.question}</p>
                </div>
              </div>

              <div className="p-4 space-y-2">
                {q.options.map((opt, oi) => {
                  const isSelected = selected === oi
                  const isRight = oi === q.correctIndex

                  let cls = 'border-gray-700 text-gray-400 hover:border-gray-500 hover:text-gray-200 cursor-pointer'
                  if (isSelected && !show) cls = 'border-blue-500 text-blue-300 bg-blue-950/30'
                  if (show && isRight) cls = 'border-green-600 text-green-300 bg-green-950/30'
                  if (show && isSelected && !isRight) cls = 'border-red-600 text-red-300 bg-red-950/30'

                  return (
                    <div
                      key={oi}
                      onClick={() => !submitted && setAnswers(a => ({ ...a, [q.id]: oi }))}
                      className={`flex items-start gap-3 p-3 rounded-lg border text-sm transition-all ${cls}`}
                    >
                      <div className={`w-5 h-5 rounded-full border-2 flex-shrink-0 mt-0.5 flex items-center justify-center transition-colors ${
                        isSelected ? 'border-current' : 'border-gray-600'
                      }`}>
                        {isSelected && !show && <div className="w-2.5 h-2.5 rounded-full bg-blue-400" />}
                        {show && isRight && <span className="text-xs">✓</span>}
                        {show && isSelected && !isRight && <span className="text-xs">✗</span>}
                      </div>
                      {opt}
                    </div>
                  )
                })}
              </div>

              {show && (
                <div className="px-6 py-4 bg-gray-800/50 border-t border-gray-800 animate-fade-in">
                  <p className="text-xs text-gray-400 leading-relaxed">
                    <span className="text-gray-300 font-medium">Explanation: </span>
                    {q.explanation}
                  </p>
                </div>
              )}
            </div>
          )
        })}
      </div>

      {/* Submit */}
      {!submitted && (
        <div className="mt-8">
          <div className="flex items-center justify-between mb-3">
            <span className="text-sm text-gray-400">{Object.keys(answers).length}/{quiz.questions.length} answered</span>
          </div>
          <button
            onClick={handleSubmit}
            disabled={!allAnswered}
            className="w-full py-3 bg-blue-600 hover:bg-blue-500 disabled:bg-gray-700 disabled:text-gray-500 text-white font-medium rounded-xl transition-colors"
          >
            {allAnswered ? 'Submit Quiz' : `Answer all questions to submit`}
          </button>
        </div>
      )}

      {submitted && (
        <div className="mt-8 flex gap-3">
          <a href={`/modules/${mod.id}`} className="flex-1 py-3 text-center bg-gray-700 hover:bg-gray-600 text-gray-200 rounded-xl transition-colors text-sm">
            ← Back to Module
          </a>
          <a href="/" className="flex-1 py-3 text-center bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-xl transition-colors text-sm">
            Dashboard
          </a>
        </div>
      )}
    </div>
  )
}
