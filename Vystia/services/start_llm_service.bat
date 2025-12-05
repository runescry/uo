@echo off
REM Vystia LLM NPC Service Startup Script for Windows
REM Starts the FastAPI service for LLM-powered NPC dialogue

echo ===========================================
echo Vystia LLM NPC Service
echo ===========================================
echo.

REM Check if Python is installed
python --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Python is not installed or not in PATH
    echo Please install Python 3.10+ from https://www.python.org
    pause
    exit /b 1
)

REM Check if virtual environment exists
if not exist "venv" (
    echo Creating virtual environment...
    python -m venv venv
)

REM Activate virtual environment
echo Activating virtual environment...
call venv\Scripts\activate.bat

REM Install/update requirements
echo Installing dependencies...
pip install -r requirements.txt --quiet

REM Check for API key
if "%OPENAI_API_KEY%"=="" (
    echo.
    echo WARNING: OPENAI_API_KEY environment variable not set!
    echo Please set it before starting the service:
    echo   set OPENAI_API_KEY=your-api-key-here
    echo.
    set /p API_KEY="Enter your OpenAI API key (or press Enter to skip): "
    if not "%API_KEY%"=="" (
        set OPENAI_API_KEY=%API_KEY%
    )
)

REM Start the service
echo.
echo ===========================================
echo Starting LLM Service on http://localhost:8000
echo ===========================================
echo.

python -m uvicorn llm_server:app --host 0.0.0.0 --port 8000 --reload

pause

