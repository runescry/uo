'use client'

import { useState, useEffect } from 'react'
import { FLASHCARDS, CATEGORIES, Flashcard } from '@/lib/flashcards'
import { loadProgress, markFlashcardReviewed } from '@/lib/progress'

export default function FlashcardsPage() {
  const [category, setCategory] = useState<string>('All')
  const [difficulty, setDifficulty] = useState<string>('All')
  const [index, setIndex] = useState(0)
  const [flipped, setFlipped] = useState(false)
  const [reviewed, setReviewed] = useState<Set<string>>(new Set())
  const [known, setKnown] = useState<Set<string>>(new Set())

  useEffect(() => {
    const p = loadProgress()
    setReviewed(new Set(p.flashcardsReviewed))
  }, [])

  const filtered = FLASHCARDS.filter(f => {
    const catOk = category === 'All' || f.category === category
    const diffOk = difficulty === 'All' || f.difficulty === difficulty
    return catOk && diffOk
  })

  const card = filtered[index]

  const handleFlip = () => setFlipped(!flipped)

  const handleKnow = (isKnown: boolean) => {
    if (!card) return
    markFlashcardReviewed(card.id)
    setReviewed(prev => new Set([...prev, card.id]))
    if (isKnown) setKnown(prev => new Set([...prev, card.id]))
    setFlipped(false)
    setTimeout(() => {
      setIndex(i => (i + 1) % filtered.length)
    }, 200)
  }

  const handlePrev = () => { setFlipped(false); setTimeout(() => setIndex(i => (i - 1 + filtered.length) % filtered.length), 100) }
  const handleNext = () => { setFlipped(false); setTimeout(() => setIndex(i => (i + 1) % filtered.length), 100) }

  const reviewedInSet = filtered.filter(f => reviewed.has(f.id)).length
  const knownInSet = filtered.filter(f => known.has(f.id)).length

  const difficultyColor: Record<string, string> = {
    foundational: 'text-green-400 bg-green-900/40 border-green-700/50',
    intermediate: 'text-yellow-400 bg-yellow-900/40 border-yellow-700/50',
    advanced: 'text-red-400 bg-red-900/40 border-red-700/50',
  }

  return (
    <div className="max-w-3xl mx-auto px-4 sm:px-6 py-8">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-white mb-1">Flashcards</h1>
        <p className="text-gray-400 text-sm">{FLASHCARDS.length} terms across {CATEGORIES.length} categories</p>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap gap-3 mb-6">
        <div className="flex flex-wrap gap-1.5">
          {['All', ...CATEGORIES].map(cat => (
            <button
              key={cat}
              onClick={() => { setCategory(cat); setIndex(0); setFlipped(false) }}
              className={`text-xs px-2.5 py-1 rounded-lg border transition-colors ${
                category === cat
                  ? 'bg-purple-600 border-purple-500 text-white'
                  : 'bg-gray-800 border-gray-700 text-gray-400 hover:text-gray-200'
              }`}
            >
              {cat}
            </button>
          ))}
        </div>
        <div className="flex gap-1.5">
          {['All', 'foundational', 'intermediate', 'advanced'].map(d => (
            <button
              key={d}
              onClick={() => { setDifficulty(d); setIndex(0); setFlipped(false) }}
              className={`text-xs px-2.5 py-1 rounded-lg border transition-colors capitalize ${
                difficulty === d
                  ? 'bg-gray-600 border-gray-500 text-white'
                  : 'bg-gray-800 border-gray-700 text-gray-400 hover:text-gray-200'
              }`}
            >
              {d}
            </button>
          ))}
        </div>
      </div>

      {/* Progress */}
      <div className="flex items-center gap-4 mb-6 text-sm">
        <span className="text-gray-400">{index + 1} / {filtered.length}</span>
        <div className="flex-1 h-1.5 bg-gray-800 rounded-full overflow-hidden">
          <div
            className="h-full bg-purple-500 rounded-full transition-all duration-300"
            style={{ width: `${((index + 1) / filtered.length) * 100}%` }}
          />
        </div>
        <span className="text-green-400 text-xs">{knownInSet} known</span>
        <span className="text-gray-500 text-xs">{reviewedInSet - knownInSet} reviewing</span>
      </div>

      {/* Card */}
      {card ? (
        <div className="mb-6">
          <div
            onClick={handleFlip}
            className="relative cursor-pointer"
            style={{ perspective: '1000px' }}
          >
            <div
              className="transition-transform duration-500 relative"
              style={{ transformStyle: 'preserve-3d', transform: flipped ? 'rotateY(180deg)' : 'rotateY(0deg)', minHeight: '220px' }}
            >
              {/* Front */}
              <div
                className="absolute inset-0 bg-gray-900 border border-purple-700/50 rounded-2xl p-8 flex flex-col items-center justify-center"
                style={{ backfaceVisibility: 'hidden' }}
              >
                <div className="text-center">
                  <div className="flex items-center gap-2 justify-center mb-4">
                    <span className={`text-xs px-2 py-0.5 rounded-full border ${difficultyColor[card.difficulty] || 'text-gray-400'}`}>
                      {card.difficulty}
                    </span>
                    <span className="text-xs text-gray-500">{card.category}</span>
                  </div>
                  <h2 className="text-2xl font-bold text-white mb-3">{card.term}</h2>
                  <p className="text-xs text-gray-500">Click to reveal definition</p>
                </div>
              </div>

              {/* Back */}
              <div
                className="absolute inset-0 bg-purple-950/30 border border-purple-600/60 rounded-2xl p-8 flex flex-col items-center justify-center"
                style={{ backfaceVisibility: 'hidden', transform: 'rotateY(180deg)' }}
              >
                <div className="text-center">
                  <h3 className="text-lg font-semibold text-purple-300 mb-4">{card.term}</h3>
                  <p className="text-gray-200 text-sm leading-relaxed">{card.definition}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      ) : (
        <div className="bg-gray-900 border border-gray-800 rounded-2xl p-8 text-center mb-6">
          <p className="text-gray-400">No cards match your filters</p>
        </div>
      )}

      {/* Controls */}
      {card && (
        <div className="space-y-3">
          {flipped ? (
            <div className="grid grid-cols-2 gap-3">
              <button
                onClick={() => handleKnow(false)}
                className="py-3 bg-red-900/40 hover:bg-red-900/60 border border-red-700/50 text-red-300 rounded-xl font-medium text-sm transition-colors"
              >
                ✗ Review Again
              </button>
              <button
                onClick={() => handleKnow(true)}
                className="py-3 bg-green-900/40 hover:bg-green-900/60 border border-green-700/50 text-green-300 rounded-xl font-medium text-sm transition-colors"
              >
                ✓ Got It
              </button>
            </div>
          ) : (
            <button
              onClick={handleFlip}
              className="w-full py-3 bg-purple-700 hover:bg-purple-600 text-white rounded-xl font-medium text-sm transition-colors"
            >
              Reveal Definition
            </button>
          )}

          <div className="flex gap-3">
            <button onClick={handlePrev} className="flex-1 py-2 bg-gray-800 hover:bg-gray-700 text-gray-400 rounded-xl text-sm transition-colors">← Prev</button>
            <button onClick={handleNext} className="flex-1 py-2 bg-gray-800 hover:bg-gray-700 text-gray-400 rounded-xl text-sm transition-colors">Next →</button>
          </div>
        </div>
      )}

      {/* All cards list */}
      <div className="mt-10">
        <h3 className="text-sm font-semibold text-gray-400 uppercase tracking-wider mb-4">All Cards in View</h3>
        <div className="space-y-2">
          {filtered.map((f, i) => (
            <div
              key={f.id}
              onClick={() => { setIndex(i); setFlipped(false) }}
              className={`flex items-start gap-3 p-3 rounded-lg border cursor-pointer transition-all ${
                i === index ? 'bg-purple-950/40 border-purple-700/50' : 'bg-gray-900 border-gray-800 hover:border-gray-700'
              }`}
            >
              <div className="flex items-center gap-2 flex-shrink-0">
                {known.has(f.id) && <span className="text-green-400 text-xs">✓</span>}
                {reviewed.has(f.id) && !known.has(f.id) && <span className="text-yellow-400 text-xs">↺</span>}
                {!reviewed.has(f.id) && <span className="text-gray-600 text-xs">○</span>}
              </div>
              <div className="flex-1 min-w-0">
                <span className="text-sm text-gray-200 font-medium">{f.term}</span>
                <span className="text-xs text-gray-600 ml-2">{f.category}</span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
