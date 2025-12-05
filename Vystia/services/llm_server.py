#!/usr/bin/env python3
"""
LLM API Bridge for Vystia NPC Dialogue System
Provides REST endpoint for ServUO to query LLM providers
"""

import os
import json
import logging
from datetime import datetime
from typing import Optional, Dict, Any
from pathlib import Path

from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
import openai
from openai import OpenAI

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[
        logging.FileHandler('logs/npc_ai_chat.log'),
        logging.StreamHandler()
    ]
)
logger = logging.getLogger(__name__)

# Create logs directory if it doesn't exist
Path('logs').mkdir(exist_ok=True)

app = FastAPI(
    title="Vystia LLM NPC Service",
    description="LLM-powered NPC dialogue for Ultima Online ServUO",
    version="1.0.0"
)

# CORS middleware for ServUO connections
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Configuration
LLM_MODEL = os.getenv("LLM_MODEL", "gpt-4o-mini")
MAX_TOKENS = int(os.getenv("MAX_TOKENS", "100"))
TEMPERATURE = float(os.getenv("TEMPERATURE", "0.7"))
API_KEY = os.getenv("OPENAI_API_KEY", "")

# Initialize OpenAI client
client = OpenAI(api_key=API_KEY) if API_KEY else None

# Load NPC profiles
PROFILES_PATH = Path("data/npc_profiles.json")
npc_profiles = {}

def load_profiles():
    """Load NPC personality profiles from JSON"""
    global npc_profiles
    if PROFILES_PATH.exists():
        with open(PROFILES_PATH, 'r') as f:
            npc_profiles = json.load(f)
        logger.info(f"Loaded {len(npc_profiles)} NPC profiles")
    else:
        logger.warning(f"Profiles file not found at {PROFILES_PATH}")
        npc_profiles = {}

load_profiles()

# Request/Response models
class LLMRequest(BaseModel):
    """Request model for LLM query"""
    npc_name: str
    npc_role: str
    player_input: str
    region: Optional[str] = None
    map_context: Optional[str] = None
    quest_state: Optional[str] = None

class LLMResponse(BaseModel):
    """Response model for LLM query"""
    response: str
    timestamp: str

def build_prompt(npc_name: str, npc_role: str, player_input: str, 
                 region: Optional[str] = None, map_context: Optional[str] = None,
                 quest_state: Optional[str] = None) -> tuple[str, str]:
    """
    Build system and user prompts for the LLM
    Returns: (system_prompt, user_prompt)
    """
    
    # Get NPC profile
    profile = npc_profiles.get(npc_role.lower(), {})
    tone = profile.get("tone", "neutral and helpful")
    context = profile.get("context", "")
    profile_region = profile.get("region", "")
    
    # Use profile region if available, otherwise use provided region
    final_region = profile_region or region or "the world of Vystia"
    
    # Build system prompt
    system_prompt = f"""You are {npc_name}, a {npc_role} in the world of Vystia.
Your personality: {tone}
Location: {final_region}
{f'Context: {context}' if context else ''}

Guidelines:
- Respond naturally and in-character
- Keep responses short (1-2 sentences)
- Stay lore-consistent with Vystia world
- Do not break character or reference AI
- Use fantasy RPG language appropriately
"""
    
    # Add quest context if available
    if quest_state:
        system_prompt += f"\nCurrent quest status: {quest_state}"
    
    # Build user prompt
    user_prompt = player_input
    
    return system_prompt, user_prompt

@app.get("/")
async def root():
    """Health check endpoint"""
    return {
        "status": "online",
        "service": "Vystia LLM NPC Service",
        "model": LLM_MODEL,
        "profiles_loaded": len(npc_profiles)
    }

@app.post("/llm", response_model=LLMResponse)
async def query_llm(request: LLMRequest):
    """
    Query the LLM for NPC dialogue response
    
    Args:
        request: LLMRequest with NPC and player context
    
    Returns:
        LLMResponse with generated dialogue
    """
    
    if not client:
        logger.error("OpenAI API key not configured")
        raise HTTPException(
            status_code=503,
            detail="LLM service not configured. Set OPENAI_API_KEY environment variable."
        )
    
    try:
        # Build prompts
        system_prompt, user_prompt = build_prompt(
            request.npc_name,
            request.npc_role,
            request.player_input,
            request.region,
            request.map_context,
            request.quest_state
        )
        
        # Log the request
        logger.info(f"[{request.npc_name}] Player: {request.player_input}")
        
        # Call OpenAI API
        response = client.chat.completions.create(
            model=LLM_MODEL,
            messages=[
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_prompt}
            ],
            max_tokens=MAX_TOKENS,
            temperature=TEMPERATURE
        )
        
        # Extract response
        reply = response.choices[0].message.content.strip()
        
        # Log response
        logger.info(f"[{request.npc_name}] NPC: {reply}")
        
        return LLMResponse(
            response=reply,
            timestamp=datetime.now().isoformat()
        )
        
    except Exception as e:
        logger.error(f"Error querying LLM: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=500,
            detail=f"Error generating response: {str(e)}"
        )

@app.get("/profiles")
async def get_profiles():
    """Get all loaded NPC profiles"""
    return {
        "profiles": npc_profiles,
        "count": len(npc_profiles)
    }

@app.post("/reload_profiles")
async def reload_profiles():
    """Reload NPC profiles from disk"""
    load_profiles()
    return {"status": "reloaded", "count": len(npc_profiles)}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)

