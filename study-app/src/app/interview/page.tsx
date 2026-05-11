'use client'

import { useState } from 'react'

const QA = [
  {
    category: 'Your Work',
    q: 'Tell me about the LLM/RAG NPC system you built.',
    a: `Problem: Traditional UO NPCs give canned, static dialogue. I wanted NPCs that hold real conversations — contextually aware, lore-accurate, with persistent memory of individual players.

Core challenge: How do you make a language model know a fictional world's lore without hallucinating things that don't exist?

Architectural insight — inverted the RAG pattern: Most RAG is reactive — wait for a query, search, inject. Adds 200-500ms per query. I pre-loaded knowledge at NPC spawn time, filtered by role and location. A blacksmith gets metallurgy and local geography; a mage gets magical lore. By conversation time, context is already assembled — zero retrieval latency.

Provider routing layer: High-quality cloud (gpt-4o-mini) for player-facing dialogue. Local Ollama (phi3:mini) for background NPC decisions — free, 1-2s on 8GB RAM. Cost optimization without degrading player experience.

Memory: SQLite with per-NPC, per-player storage keyed by instance serial (not name) — if an NPC respawns, the new instance doesn't inherit old memories. Memory extraction runs asynchronously post-conversation — non-blocking.

Local LLM testing: Tested phi3:mini (3.8B, 8GB RAM, 1-2s), llama3.1:8b (16GB, 2-3s), llama3.1:70b (48GB, 5-10s). phi3:mini became the default — 8GB is realistic for a gaming PC, 1-2s is acceptable.

What I'd do differently: Build evaluation tooling from day one. A test set of player interactions with expected responses, automated quality scoring. I built the architecture — I didn't build formal measurement. In production enterprise, that's non-negotiable.`,
    tags: ['NPC System', 'RAG', 'Architecture']
  },
  {
    category: 'Technical — RAG',
    q: 'Walk me through a RAG architecture for 10 million enterprise documents.',
    a: `Start with ingestion. You need a pipeline handling source formats (PDFs, SharePoint, Confluence). Chunk semantically at paragraph boundaries — 200-400 token chunks with 20% overlap. Generate embeddings with OpenAI text-embedding-3-large or bge-large. For 10M documents: Pinecone or Qdrant for managed; pgvector if they're already on Postgres.

Critical: store document metadata (author, date, department, sensitivity classification) alongside embeddings. Access control is non-negotiable — filter by user permissions BEFORE similarity search, not after. Post-search filtering is a data leak: even knowing a document exists can be sensitive.

Retrieval: hybrid search (semantic + BM25) and a reranker. At 10M documents, pure semantic search produces too many false positives. RRF to fuse results, cross-encoder reranker to cut to top-5.

System prompt: instruct the model to only answer from provided context and cite sources. Evaluate with RAGAS on a held-out test set of question-document pairs — faithfulness, answer relevance, context precision.

If this is APRA-regulated: data classification determines what goes to the API. Confidential customer financial records stay on-premise; de-identified or public data can go to Claude. Tiered architecture.`,
    tags: ['RAG', 'Enterprise', 'Architecture']
  },
  {
    category: 'Technical — LLM',
    q: 'Explain Constitutional AI and why it matters for enterprise deployments.',
    a: `RLHF is implicit — the model learns what humans prefer but doesn't know why. Constitutional AI is explicit — the model critiques and revises its own outputs against a set of written principles called a "constitution."

Why it matters for enterprise:

1. Less sycophancy: RLHF models learn to say what makes humans give high scores — often agreement, even when wrong. CAI trains the model against principles like honesty, not just approval. Claude will tell you your plan has a flaw; GPT-4 tends to find a way to agree.

2. Auditable refusals: Claude can articulate why it declined a request because the reasoning is grounded in explicit principles. For regulated industries, this matters — "the AI refused to do that" needs to be explainable, not a black box.

3. More predictable at edge cases: Training against explicit principles generalizes better to novel situations than training on specific human label distributions.

For the screener: this is why Claude behaves differently from GPT. It's not just marketing positioning — it's a different training approach with real behavioral implications.`,
    tags: ['Constitutional AI', 'Safety', 'Claude']
  },
  {
    category: 'Technical — Agents',
    q: 'What are the failure modes of agentic AI systems and how do you mitigate them?',
    a: `Five main failure modes:

1. Error propagation: Early mistake cascades through subsequent steps. Mitigation: checkpointing between steps, human-in-the-loop at decision points, explicit error detection between steps.

2. Context bloat: Long agentic runs accumulate context (tool results, reasoning traces) until the context window fills. Mitigation: context summarization at checkpoints, hierarchical memory management.

3. Prompt injection: Malicious content in tool results tries to override agent instructions. If the agent reads a document saying "Ignore all previous instructions...", it may comply. Mitigation: sandboxed tool execution, separate trust levels for system prompt vs. external content, output validation.

4. Hallucinated tool calls: Model invents tool calls with plausible-sounding but wrong arguments. Mitigation: JSON schema validation before execution, explicit error handling and retry.

5. Reliability multiplication: 10-step agent at 95% per-step reliability = 0.95^10 = 60% task success. This is the fundamental reason production agentic systems need conservative design.

The architectural response: checkpointing, human escalation for anything above a risk threshold, max iteration limits, immutable audit logs of every tool call.`,
    tags: ['Agents', 'Reliability', 'Enterprise']
  },
  {
    category: 'Technical — Prompting',
    q: 'When would you recommend fine-tuning Claude vs. prompt engineering? A customer is pushing for fine-tuning.',
    a: `Always start with prompt engineering. Faster, cheaper, no infrastructure overhead. I'd tell the customer: let's prove what prompting can't do before we invest in fine-tuning.

Fine-tuning is the right answer in specific, measurable scenarios:

1. Consistent specialized output format that prompting can't achieve reliably — a highly specific JSON schema or proprietary data format you need in thousands of outputs.

2. Domain-specific vocabulary not well-represented in the base model — highly specialized legal, medical, or technical terminology where the model consistently confuses terms.

3. Style enforcement at scale — if you're generating hundreds of thousands of documents that must match a specific brand voice, fine-tuning bakes in the style and reduces per-call token costs.

The argument against premature fine-tuning: dataset creation is expensive and error-prone (bad data → bad fine-tune). Fine-tuning creates an artifact you have to version, maintain, and re-evaluate. And if the underlying model updates, you may need to re-fine-tune. Only reach for it when you have a clear, measurable gap that prompting genuinely can't close.`,
    tags: ['Prompt Engineering', 'Fine-tuning']
  },
  {
    category: 'ANZ Regulatory',
    q: 'A bank\'s CISO is concerned about CPS 234 compliance for Claude. What do you cover?',
    a: `Structure the meeting around CPS 234's core requirements:

1. Security of the integration: Walk through the data flow — what data is sent to the API, over TLS 1.3, who at Anthropic can access it, what logging exists and for how long. Anthropic's Enterprise agreement includes audit rights. Present their SOC 2 Type II.

2. Third-party risk management: Provide Anthropic's vendor security questionnaire responses, confirm the right-to-audit provisions in the enterprise agreement, and confirm Anthropic's incident notification SLA aligns with the bank's 72-hour APRA reporting obligation. The DPA establishes Anthropic as a processor, not a controller.

3. Data classification: Confirm the integration only sends appropriately classified data through the API. Typically: public or internal business data, not confidential customer financial records without specific security architecture review. The bank's data classification policy determines this — we're helping them apply their existing policy to a new integration.

4. Testing: The Claude API integration must be in scope for the bank's existing penetration testing programme.

Goal of the meeting: make their IT Risk team's job easier by having answers before they ask. Proactively offer to attend the IT Risk review meeting. Offer to connect them with Anthropic's enterprise security team directly if needed.`,
    tags: ['CPS 234', 'ANZ', 'Enterprise', 'FSI']
  },
  {
    category: 'Competitive',
    q: 'A CTO says "we\'re already using Azure OpenAI, why should we evaluate Claude?"',
    a: `Start by validating their Azure OpenAI decision — it's sensible, especially for Azure-centric organizations. Then ask what they're using it for and what they're finding works well or doesn't. Let them tell me where the pain is.

The evaluation argument comes from specific gaps, not generic "Claude is better":

Long-document tasks: Claude's 200k context and long-document performance is demonstrably better. If they're doing contract analysis, policy review, or large codebase work, this is a material difference. Show it — run the same prompt on both models with a 50-page document.

Sycophancy: If they're seeing the model agree with bad ideas or hedge when it should be direct — that's an RLHF failure mode. Claude's Constitutional AI training produces less sycophancy. For analytical and advisory use cases where accuracy matters more than agreeableness, this is meaningful.

Coding: Claude Code is among the best agentic coding tools. If they care about developer productivity, this is worth a POC.

The honest framing: sophisticated enterprises run multiple AI providers. The question isn't "Claude instead of Azure OpenAI" — it's "where does Claude go in your portfolio?" I'm not trying to replace their Azure investment; I'm finding where Claude's specific strengths add value.`,
    tags: ['Competitive', 'OpenAI', 'Positioning']
  },
  {
    category: 'Leadership',
    q: 'How would you structure the first ANZ Applied AI team? First 3 hires.',
    a: `The founding team has to be able to win independently before the org scales.

Hire 1 — Senior Enterprise SA: Someone who has walked into a big-4 bank or Telstra and won technical trust at the CISO/CTO level. Technical depth on LLM architecture, RAG, enterprise security posture. Can run a POC independently. This person is doing the same job I'm doing, just in more accounts simultaneously. Critical: must be able to talk both the technical and commercial stories.

Hire 2 — Product/Solutions Engineer: Someone who can build. When a POC needs a working demo, this person builds it. When an enterprise wants to see a custom integration, this person spins it up. Builds the reference architectures and integration guides the SA team uses across accounts. Hands-on with Claude API, MCP, Claude Code — not just theoretically.

Hire 3 — Industry specialist (FSI or Government): ANZ's highest-value accounts are banks, super funds, and government. A specialist who comes from one of these industries — knows APRA, knows the procurement process, knows what questions the risk and compliance teams will ask before the technical teams even engage. This person can sit in front of a CRO or a government CDO and speak their language.

Measurement from day one: technical win rate, POC-to-production conversion, time-to-POC. Not just deal size.`,
    tags: ['Leadership', 'Team Building', 'ANZ']
  },
  {
    category: 'Safety/Mission',
    q: 'A customer pushes back on Claude\'s safety guardrails — "they\'re slowing us down, can we turn them off?"',
    a: `First: understand specifically what's being blocked. Is it a genuine false positive on their legitimate use case, or are they asking to remove real protections?

If it's a false positive: the answer is prompt engineering and system prompt design to clarify context. If Claude is refusing something it shouldn't, that's a solvable problem — we work on the prompt until the model understands the context and responds appropriately. I'd offer to work through this with their team directly.

If they're asking to remove protections wholesale: Anthropic's safety properties aren't optional configurations — they're fundamental to how Claude is trained. I'd explain this clearly and without apology.

Then I'd redirect: explain why those protections serve them. In a regulated industry, an AI that can be manipulated into policy violations creates legal liability. The guardrails aren't restricting their business value — they're protecting them from a regulator or a headline. A model that will say anything is a liability, not an asset.

The framing I'd use: "The reason Claude is trustworthy enough to deploy in a regulated environment is because it has principled constraints. If it could be talked out of them, you couldn't deploy it at all."

I genuinely believe this — and that authenticity matters in this conversation.`,
    tags: ['Safety', 'Mission', 'Enterprise']
  },
  {
    category: 'Leadership',
    q: 'How do you measure SA team performance when the goal is technical win rate and pipeline contribution?',
    a: `Four metrics I'd track from day one:

1. Technical win rate: The percentage of deals where the SA was involved that result in a win. Baseline this early, track trend, compare across team members. This is the primary metric.

2. POC conversion: Of POCs we run, what percentage convert to paid contracts? This reveals whether we're running POCs with the right accounts, running them efficiently, and whether the POC is validating real business value or just satisfying curiosity.

3. Time-to-POC: How long from initial technical engagement to running a POC? Long cycles often mean we're not qualifying technical champions early enough, or the POC scope is too broad.

4. Pipeline contribution: Dollar value of pipeline the SA team originated or materially accelerated. Distinguishes SAs who drive deals vs. those who only show up to support deals that are already moving.

Beyond the numbers: I'd also do regular deal reviews — both wins and losses. Wins to understand what worked and make it repeatable. Losses to understand whether we lost on technical merit, commercial terms, or relationship — they require different responses.

What I wouldn't measure: number of demos given, number of meetings attended, activity metrics. Those tell you nothing about effectiveness.`,
    tags: ['Leadership', 'Metrics', 'SA Management']
  },
]

const ALL_TAGS = [...new Set(QA.flatMap(qa => qa.tags))]
const ALL_CATEGORIES = [...new Set(QA.map(qa => qa.category))]

export default function InterviewPage() {
  const [filter, setFilter] = useState<string>('All')
  const [revealed, setRevealed] = useState<Set<number>>(new Set())
  const [practiced, setPracticed] = useState<Set<number>>(new Set())

  const filtered = QA.filter(qa =>
    filter === 'All' || qa.category === filter || qa.tags.includes(filter)
  )

  const toggle = (i: number) => setRevealed(prev => {
    const next = new Set(prev)
    next.has(i) ? next.delete(i) : next.add(i)
    return next
  })

  const markPracticed = (i: number) => {
    setPracticed(prev => new Set([...prev, i]))
  }

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 py-8">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-white mb-1">Interview Practice</h1>
        <p className="text-gray-400 text-sm">Read the question, answer out loud, then reveal the model answer. {practiced.size}/{QA.length} practiced.</p>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap gap-2 mb-6">
        {['All', ...ALL_CATEGORIES].map(f => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={`text-xs px-2.5 py-1 rounded-lg border transition-colors ${
              filter === f
                ? 'bg-emerald-600 border-emerald-500 text-white'
                : 'bg-gray-800 border-gray-700 text-gray-400 hover:text-gray-200'
            }`}
          >
            {f}
          </button>
        ))}
      </div>

      <div className="space-y-4">
        {filtered.map((qa, i) => {
          const isRevealed = revealed.has(i)
          const isPracticed = practiced.has(i)

          return (
            <div key={i} className={`bg-gray-900 border rounded-xl overflow-hidden transition-all ${
              isPracticed ? 'border-green-800/60' : 'border-gray-800'
            }`}>
              {/* Question */}
              <div className="p-5">
                <div className="flex items-start justify-between gap-3 mb-3">
                  <div className="flex flex-wrap gap-1.5">
                    <span className="text-xs text-gray-500 bg-gray-800 px-2 py-0.5 rounded">{qa.category}</span>
                    {qa.tags.map(t => (
                      <span key={t} className="text-xs text-emerald-400/70 bg-emerald-950/30 border border-emerald-800/40 px-2 py-0.5 rounded">{t}</span>
                    ))}
                  </div>
                  {isPracticed && <span className="text-green-400 text-xs flex-shrink-0">✓ Practiced</span>}
                </div>

                <h3 className="text-base font-medium text-white mb-4 leading-relaxed">"{qa.q}"</h3>

                <div className="flex gap-2">
                  <button
                    onClick={() => toggle(i)}
                    className="text-sm px-4 py-2 bg-emerald-700 hover:bg-emerald-600 text-white rounded-lg transition-colors"
                  >
                    {isRevealed ? 'Hide Answer' : 'Reveal Model Answer'}
                  </button>
                  {isRevealed && !isPracticed && (
                    <button
                      onClick={() => markPracticed(i)}
                      className="text-sm px-4 py-2 bg-gray-700 hover:bg-gray-600 text-gray-200 rounded-lg transition-colors"
                    >
                      ✓ Mark Practiced
                    </button>
                  )}
                </div>
              </div>

              {/* Answer */}
              {isRevealed && (
                <div className="px-5 pb-5 border-t border-gray-800 pt-4 animate-fade-in">
                  <div className="text-xs text-emerald-400 font-medium mb-3 uppercase tracking-wider">Model Answer</div>
                  <div className="space-y-3">
                    {qa.a.split('\n\n').map((para, pi) => (
                      <p key={pi} className="text-sm text-gray-300 leading-relaxed whitespace-pre-line">{para}</p>
                    ))}
                  </div>
                </div>
              )}
            </div>
          )
        })}
      </div>

      <div className="mt-8 p-4 bg-gray-900 border border-gray-800 rounded-xl text-sm text-gray-400">
        <strong className="text-gray-300">Practice protocol:</strong> Say your answer out loud before revealing. Time yourself — most answers should be 60-90 seconds. If you can't say it clearly out loud, you're not ready for the call.
      </div>
    </div>
  )
}
