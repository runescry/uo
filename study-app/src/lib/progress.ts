'use client'

export interface ModuleProgress {
  moduleId: string
  completed: boolean
  quizScore?: number
  quizAttempted: boolean
  lastVisited?: string
}

export interface StudyProgress {
  modules: Record<string, ModuleProgress>
  flashcardsReviewed: string[]
  interviewQuestionsReviewed: string[]
}

const STORAGE_KEY = 'anthropic-study-progress'

function getDefaultProgress(): StudyProgress {
  return { modules: {}, flashcardsReviewed: [], interviewQuestionsReviewed: [] }
}

export function loadProgress(): StudyProgress {
  if (typeof window === 'undefined') return getDefaultProgress()
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? JSON.parse(raw) : getDefaultProgress()
  } catch {
    return getDefaultProgress()
  }
}

export function saveProgress(progress: StudyProgress): void {
  if (typeof window === 'undefined') return
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(progress))
  } catch { /* ignore */ }
}

export function markModuleVisited(moduleId: string): void {
  const p = loadProgress()
  if (!p.modules[moduleId]) {
    p.modules[moduleId] = { moduleId, completed: false, quizAttempted: false }
  }
  p.modules[moduleId].lastVisited = new Date().toISOString()
  saveProgress(p)
}

export function markModuleCompleted(moduleId: string): void {
  const p = loadProgress()
  if (!p.modules[moduleId]) {
    p.modules[moduleId] = { moduleId, completed: false, quizAttempted: false }
  }
  p.modules[moduleId].completed = true
  saveProgress(p)
}

export function saveQuizScore(moduleId: string, score: number): void {
  const p = loadProgress()
  if (!p.modules[moduleId]) {
    p.modules[moduleId] = { moduleId, completed: false, quizAttempted: false }
  }
  p.modules[moduleId].quizAttempted = true
  p.modules[moduleId].quizScore = Math.max(score, p.modules[moduleId].quizScore ?? 0)
  saveProgress(p)
}

export function markFlashcardReviewed(cardId: string): void {
  const p = loadProgress()
  if (!p.flashcardsReviewed.includes(cardId)) {
    p.flashcardsReviewed.push(cardId)
  }
  saveProgress(p)
}

export function resetProgress(): void {
  if (typeof window === 'undefined') return
  localStorage.removeItem(STORAGE_KEY)
}

export function getOverallStats(progress: StudyProgress, totalModules: number, totalCards: number) {
  const completed = Object.values(progress.modules).filter(m => m.completed).length
  const quizzesTaken = Object.values(progress.modules).filter(m => m.quizAttempted).length
  const avgScore = quizzesTaken > 0
    ? Object.values(progress.modules)
        .filter(m => m.quizAttempted && m.quizScore !== undefined)
        .reduce((sum, m) => sum + (m.quizScore ?? 0), 0) / quizzesTaken
    : 0
  return {
    modulesCompleted: completed,
    totalModules,
    completionPercent: Math.round((completed / totalModules) * 100),
    quizzesTaken,
    avgQuizScore: Math.round(avgScore),
    flashcardsReviewed: progress.flashcardsReviewed.length,
    totalCards,
  }
}
