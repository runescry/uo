export interface Flashcard {
  id: string
  term: string
  definition: string
  category: string
  difficulty: 'foundational' | 'intermediate' | 'advanced'
}

export const FLASHCARDS: Flashcard[] = [
  // LLM Fundamentals
  { id: 'f1', term: 'Token', definition: 'The atomic unit of LLM computation. ~0.75 words or ~4 characters on average. Determines cost and latency.', category: 'LLM Fundamentals', difficulty: 'foundational' },
  { id: 'f2', term: 'Context window', definition: 'How many tokens the model can see at once during inference. Working memory — not persistent. Claude: 200k tokens.', category: 'LLM Fundamentals', difficulty: 'foundational' },
  { id: 'f3', term: 'Self-attention', definition: 'Mechanism that lets each token attend to every other token in the context, enabling long-range dependency understanding.', category: 'LLM Fundamentals', difficulty: 'intermediate' },
  { id: 'f4', term: 'Temperature', definition: 'Controls output randomness. 0 = deterministic (always pick highest-probability token). 1 = full distribution sampling.', category: 'LLM Fundamentals', difficulty: 'foundational' },
  { id: 'f5', term: 'RLHF', definition: 'Reinforcement Learning from Human Feedback. Humans rank responses → train reward model → optimize LLM with RL. Alignment via implicit preferences.', category: 'LLM Fundamentals', difficulty: 'intermediate' },
  { id: 'f6', term: 'Constitutional AI (CAI)', definition: 'Anthropic\'s alignment approach. Model critiques/revises its outputs against explicit written principles. Transparent, auditable, less sycophantic than RLHF.', category: 'LLM Fundamentals', difficulty: 'intermediate' },
  { id: 'f7', term: 'Top-p (nucleus sampling)', definition: 'Only sample from smallest set of tokens whose cumulative probability exceeds p. Filters out low-probability noise while preserving diversity.', category: 'LLM Fundamentals', difficulty: 'intermediate' },
  { id: 'f8', term: 'Prompt caching', definition: 'Cache frequently-used prompt prefixes (system prompt, large doc context). Anthropic charges ~90% less for cache hits. Critical for production cost optimization.', category: 'LLM Fundamentals', difficulty: 'intermediate' },
  { id: 'f9', term: 'RoPE (Rotary Position Embedding)', definition: 'Positional encoding used in modern LLMs. Handles long contexts better than original absolute positional encodings. Enables reliable 200k+ token contexts.', category: 'LLM Fundamentals', difficulty: 'advanced' },
  { id: 'f10', term: 'Pretraining vs. Fine-tuning', definition: 'Pretraining: learn language from massive corpus (next-token prediction). Fine-tuning: adapt a pretrained model for specific task/style using smaller dataset.', category: 'LLM Fundamentals', difficulty: 'foundational' },

  // Claude & Products
  { id: 'f11', term: 'Claude Haiku', definition: 'Fastest, cheapest Claude model (~$0.25/M tokens). Best for high-volume simple tasks: classification, extraction, routing.', category: 'Claude & Products', difficulty: 'foundational' },
  { id: 'f12', term: 'Claude Sonnet', definition: 'Balanced Claude model (~$3/M tokens). Primary production model for most enterprise workloads. Strong reasoning + good speed.', category: 'Claude & Products', difficulty: 'foundational' },
  { id: 'f13', term: 'Claude Opus', definition: 'Most capable Claude model (~$15/M tokens). Complex multi-step reasoning, long-document analysis, frontier-capability tasks.', category: 'Claude & Products', difficulty: 'foundational' },
  { id: 'f14', term: 'MCP (Model Context Protocol)', definition: 'Anthropic\'s open standard for connecting AI models to external tools and data. Like USB-C for AI — build once, reuse across all MCP-compatible clients.', category: 'Claude & Products', difficulty: 'intermediate' },
  { id: 'f15', term: 'System prompt', definition: 'Persistent instructions sent separately from conversation history. Sets persona, constraints, RAG context. Applies to all responses in the session.', category: 'Claude & Products', difficulty: 'foundational' },
  { id: 'f16', term: 'Tool use / Function calling', definition: 'Claude generates structured requests to call external functions. Application code executes the function and returns results. The boundary where safety controls live.', category: 'Claude & Products', difficulty: 'intermediate' },
  { id: 'f17', term: 'Enterprise DPA', definition: 'Data Processing Addendum. Contractually commits Anthropic to NOT training on enterprise customer data. The answer to "will Anthropic train on our data?"', category: 'Claude & Products', difficulty: 'foundational' },
  { id: 'f18', term: 'Extended thinking', definition: 'Configurable reasoning token budget for Claude. Claude reasons through complex problems before responding. More budget = better performance, higher latency.', category: 'Claude & Products', difficulty: 'intermediate' },

  // RAG
  { id: 'f19', term: 'RAG', definition: 'Retrieval-Augmented Generation. Provide information to the model at inference time via retrieval, rather than relying on training knowledge.', category: 'RAG', difficulty: 'foundational' },
  { id: 'f20', term: 'Chunking', definition: 'Splitting documents into pieces for embedding and retrieval. Fixed-size, semantic (boundary-aware), or hierarchical. Chunk size tradeoff: precision vs. context.', category: 'RAG', difficulty: 'foundational' },
  { id: 'f21', term: 'Embedding', definition: 'Dense vector representation of semantic content. Similar meaning → similar vectors. Generated by a separate embedding model (not Claude).', category: 'RAG', difficulty: 'foundational' },
  { id: 'f22', term: 'Cosine similarity', definition: 'Angle between two vectors. 1.0 = identical direction (same meaning), 0 = unrelated, -1 = opposite. Standard metric for semantic search.', category: 'RAG', difficulty: 'intermediate' },
  { id: 'f23', term: 'Hybrid search', definition: 'Combines semantic (vector) + keyword (BM25) search. Fused with Reciprocal Rank Fusion (RRF). Outperforms either approach alone for most production RAG.', category: 'RAG', difficulty: 'intermediate' },
  { id: 'f24', term: 'Reranking', definition: 'Second-pass scoring of retrieved results using a cross-encoder model. Trades speed for precision. Dramatically improves RAG result quality.', category: 'RAG', difficulty: 'intermediate' },
  { id: 'f25', term: 'HyDE', definition: 'Hypothetical Document Embeddings. Generate a hypothetical answer to the query, embed that, use for retrieval. The answer "looks more like" documents than the question does.', category: 'RAG', difficulty: 'advanced' },
  { id: 'f26', term: 'RAGAS', definition: 'RAG Assessment framework. Measures: faithfulness (no hallucination), answer relevance, context precision, context recall. Standard RAG evaluation tool.', category: 'RAG', difficulty: 'intermediate' },
  { id: 'f27', term: 'Lost in the middle', definition: 'Models pay less attention to content in positions 3-7 of a long context. Put important retrieved chunks at the START or END of the context block.', category: 'RAG', difficulty: 'intermediate' },
  { id: 'f28', term: 'Proactive RAG', definition: 'Pre-load role/context-appropriate knowledge at session start. Zero per-query retrieval latency. Tradeoff: less precise than per-query retrieval.', category: 'RAG', difficulty: 'intermediate' },

  // Agents
  { id: 'f29', term: 'ReAct pattern', definition: 'Reasoning + Acting. Agent loop: Think → Act → Observe → Think → Act → ... Standard agentic architecture.', category: 'Agents', difficulty: 'intermediate' },
  { id: 'f30', term: 'Prompt injection', definition: 'Malicious content in external sources (documents, web) that tries to override agent instructions. "Ignore all previous instructions..." in a document the agent reads.', category: 'Agents', difficulty: 'intermediate' },
  { id: 'f31', term: 'Orchestrator-subagent', definition: 'One orchestrator agent breaks tasks into subtasks, delegates to specialist subagents. Orchestrator manages state; subagents handle specific domains.', category: 'Agents', difficulty: 'intermediate' },
  { id: 'f32', term: 'Agent reliability math', definition: '10-step agent at 95% per-step reliability: 0.95^10 ≈ 60% task success. Multi-step agents need checkpointing, human escalation, conservative tool design.', category: 'Agents', difficulty: 'intermediate' },
  { id: 'f33', term: 'Chain vs. Agent', definition: 'Chain: fixed predetermined sequence. Agent: LLM decides what steps to take. Use chains when workflow is known; agents when path depends on what\'s found.', category: 'Agents', difficulty: 'foundational' },

  // Prompt Engineering
  { id: 'f34', term: 'Zero-shot prompting', definition: 'Just ask — no examples. Works for well-known tasks where the model has strong priors. Starting point before adding few-shot examples.', category: 'Prompt Engineering', difficulty: 'foundational' },
  { id: 'f35', term: 'Few-shot prompting', definition: 'Provide 3-5 (input, ideal output) examples before the actual request. Shows the model what "good" looks like. Most effective prompt technique.', category: 'Prompt Engineering', difficulty: 'foundational' },
  { id: 'f36', term: 'Chain-of-Thought (CoT)', definition: '"Let\'s think step by step." Causes the model to externalize reasoning. Significantly improves arithmetic, logic, and multi-step problem performance.', category: 'Prompt Engineering', difficulty: 'foundational' },
  { id: 'f37', term: 'LLM-as-judge', definition: 'Use a stronger model to score another model\'s outputs. Scales evaluation cheaply. Risk: judge model has its own biases. Validate with human eval.', category: 'Prompt Engineering', difficulty: 'intermediate' },
  { id: 'f38', term: 'Sycophancy', definition: 'Model tells users what they want to hear rather than the truth. RLHF failure mode. Claude\'s Constitutional AI training is designed to reduce this.', category: 'Prompt Engineering', difficulty: 'intermediate' },

  // Enterprise Architecture
  { id: 'f39', term: 'Semantic caching', definition: 'Cache full LLM responses for similar queries. New query → embed → check cosine similarity → return cache if match. Risk: stale cached responses.', category: 'Enterprise', difficulty: 'intermediate' },
  { id: 'f40', term: 'Model routing', definition: 'Route simple queries to Haiku, complex to Sonnet/Opus. 10x cost savings on simple traffic. Build a classifier to determine task complexity.', category: 'Enterprise', difficulty: 'intermediate' },
  { id: 'f41', term: 'Human-in-the-loop', definition: 'AI generates → confidence check → above threshold: auto-approve, below: escalate to human. AI handles volume; humans handle uncertainty.', category: 'Enterprise', difficulty: 'foundational' },
  { id: 'f42', term: 'Guardrails', definition: 'Input guardrails (filter off-topic, PII, injection) + output guardrails (check for PII leakage, policy violations, hallucinations). Layer around every production LLM call.', category: 'Enterprise', difficulty: 'intermediate' },

  // ANZ Regulatory
  { id: 'f43', term: 'APP 6 (Secondary use)', definition: 'Using personal information for a purpose other than primary collection purpose requires consent. Training AI on customer support transcripts = secondary use.', category: 'ANZ Regulatory', difficulty: 'intermediate' },
  { id: 'f44', term: 'APP 11 (Security)', definition: 'Reasonable steps to protect personal information. For AI: encryption in transit, access controls, audit logging. Applies to all AI processing of personal data.', category: 'ANZ Regulatory', difficulty: 'intermediate' },
  { id: 'f45', term: 'CPS 234', definition: 'APRA\'s information security standard. Requires third-party risk assessment for providers like Anthropic. 72-hour incident notification to APRA.', category: 'ANZ Regulatory', difficulty: 'intermediate' },
  { id: 'f46', term: 'Notifiable data breach', definition: 'Under the Privacy Act, a breach likely to cause serious harm must be reported to OAIC and affected individuals. LLM leaking one customer\'s data to another = notifiable.', category: 'ANZ Regulatory', difficulty: 'foundational' },
  { id: 'f47', term: 'Data sovereignty', definition: 'Data must be processed/stored within Australian borders. Claude API processes in the US — not suitable for strict sovereignty requirements without architectural mitigation.', category: 'ANZ Regulatory', difficulty: 'foundational' },

  // AI Safety
  { id: 'f48', term: 'Alignment', definition: 'Ensuring AI systems do what humans actually want, not just what they\'re literally instructed to do. Core research area at Anthropic.', category: 'AI Safety', difficulty: 'foundational' },
  { id: 'f49', term: 'Interpretability', definition: 'Understanding what\'s happening inside a neural network. Anthropic priority: find structures inside models that correspond to human-understandable concepts.', category: 'AI Safety', difficulty: 'intermediate' },
  { id: 'f50', term: 'RSP (Responsible Scaling Policy)', definition: 'Anthropic\'s commitment to defined safety evaluations before training/deploying more capable models. Concrete and auditable — not marketing.', category: 'AI Safety', difficulty: 'intermediate' },
  { id: 'f51', term: 'Claude\'s priority ordering', definition: 'Broadly Safe → Broadly Ethical → Anthropic\'s Principles → Genuinely Helpful. Safety is the first property, not a constraint on helpfulness.', category: 'AI Safety', difficulty: 'foundational' },
  { id: 'f52', term: 'Red-teaming', definition: 'Adversarial testing where humans try to make the model produce harmful outputs. Anthropic does this before every major release. Enterprise deployments should too.', category: 'AI Safety', difficulty: 'intermediate' },

  // Vectors
  { id: 'f53', term: 'pgvector', definition: 'Postgres extension that adds vector search capabilities. Best choice for enterprises already on Postgres — no new infra, existing compliance tooling. Scales to ~10M vectors.', category: 'Vector Databases', difficulty: 'intermediate' },
  { id: 'f54', term: 'BM25', definition: 'Best Match 25. Keyword-based ranking algorithm used in hybrid search alongside vector search. Handles exact matches (product codes, proper nouns) better than semantic search.', category: 'Vector Databases', difficulty: 'intermediate' },
  { id: 'f55', term: 'RRF (Reciprocal Rank Fusion)', definition: 'Algorithm for combining results from multiple retrieval methods (vector + keyword). Standard fusion method in hybrid search RAG systems.', category: 'Vector Databases', difficulty: 'advanced' },
]

export const CATEGORIES = [...new Set(FLASHCARDS.map(f => f.category))]
