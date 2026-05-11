import type { Metadata } from 'next'
import './globals.css'

export const metadata: Metadata = {
  title: 'Anthropic Interview Study App',
  description: 'Deep study system for LLM, RAG, Agents, SDLC, and enterprise AI',
}

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body className="min-h-screen bg-[#0a0f1e] text-gray-100">
        <nav className="border-b border-gray-800 bg-[#0d1424] sticky top-0 z-50">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex items-center justify-between h-14">
              <a href="/" className="flex items-center gap-3">
                <div className="w-7 h-7 bg-gradient-to-br from-violet-500 to-blue-500 rounded-lg flex items-center justify-center text-xs font-bold">A</div>
                <span className="font-semibold text-gray-100 text-sm hidden sm:block">Applied AI Study</span>
              </a>
              <div className="flex items-center gap-1">
                <a href="/" className="px-3 py-1.5 text-sm text-gray-400 hover:text-gray-100 hover:bg-gray-800 rounded-md transition-colors">Modules</a>
                <a href="/flashcards" className="px-3 py-1.5 text-sm text-gray-400 hover:text-gray-100 hover:bg-gray-800 rounded-md transition-colors">Flashcards</a>
                <a href="/interview" className="px-3 py-1.5 text-sm text-gray-400 hover:text-gray-100 hover:bg-gray-800 rounded-md transition-colors">Interview</a>
              </div>
            </div>
          </div>
        </nav>
        <main>{children}</main>
      </body>
    </html>
  )
}
