"""
Mass Mage Balance Tester
Runs hundreds of 1v1 simulations between all 12 magic schools
Generates comprehensive balance reports

Usage:
  python mass_mage_balance_test.py [--iterations 100]
"""

import sys
import os
sys.path.append(r"C:\DevEnv\GIT\UO\ServUO\Scripts\Services\AISidekicks\Simulation")

import argparse
import json
from collections import defaultdict
from datetime import datetime
from mage_spell_data import load_all_schools
from mage_combat_simulator import simulate_mage_combat

def run_mass_simulation(iterations_per_matchup=100):
    """Run all school vs school matchups"""
    print("="*80)
    print("VYSTIA MAGE BALANCE MASS SIMULATION")
    print("="*80)
    print(f"Iterations per matchup: {iterations_per_matchup}")
    print(f"Loading magic schools...")

    schools = load_all_schools()
    school_list = list(schools.values())

    print(f"Loaded {len(school_list)} schools")
    print("="*80)

    # Statistics tracking
    matchup_stats = defaultdict(lambda: {
        "wins": 0,
        "losses": 0,
        "total_damage_dealt": 0,
        "total_healing_done": 0,
        "total_duration": 0,
        "avg_hp_remaining": 0
    })

    school_overall_stats = defaultdict(lambda: {
        "total_wins": 0,
        "total_losses": 0,
        "win_rate": 0.0,
        "avg_ttk": 0.0,  # time to kill
        "matchups": {}
    })

    total_matches = 0
    completed_matches = 0

    # Calculate total matches
    for i, school1 in enumerate(school_list):
        for j, school2 in enumerate(school_list):
            if i != j:  # Don't fight yourself
                total_matches += iterations_per_matchup

    print(f"Total matches to simulate: {total_matches}")
    print(f"Starting simulation...\n")

    # Run all matchups
    for i, school1 in enumerate(school_list):
        for j, school2 in enumerate(school_list):
            if i == j:
                continue  # Skip mirror matches

            matchup_name = f"{school1.name} vs {school2.name}"
            print(f"Testing: {matchup_name} ({iterations_per_matchup} iterations)...", end=" ")

            for iteration in range(iterations_per_matchup):
                # Run simulation
                result = simulate_mage_combat(school1, school2, verbose=False)

                # Track stats
                if result["winner"] == school1.name:
                    matchup_stats[matchup_name]["wins"] += 1
                    school_overall_stats[school1.name]["total_wins"] += 1
                    school_overall_stats[school2.name]["total_losses"] += 1
                else:
                    matchup_stats[matchup_name]["losses"] += 1
                    school_overall_stats[school2.name]["total_wins"] += 1
                    school_overall_stats[school1.name]["total_losses"] += 1

                matchup_stats[matchup_name]["total_damage_dealt"] += result["winner_damage_dealt"]
                matchup_stats[matchup_name]["total_healing_done"] += result["winner_healing_done"]
                matchup_stats[matchup_name]["total_duration"] += result["duration"]
                matchup_stats[matchup_name]["avg_hp_remaining"] += result["winner_hp_remaining"]

                completed_matches += 1

            # Calculate averages
            stats = matchup_stats[matchup_name]
            stats["win_rate"] = (stats["wins"] / iterations_per_matchup) * 100
            stats["avg_damage"] = stats["total_damage_dealt"] / iterations_per_matchup
            stats["avg_healing"] = stats["total_healing_done"] / iterations_per_matchup
            stats["avg_duration"] = stats["total_duration"] / iterations_per_matchup
            stats["avg_hp_remaining"] = stats["avg_hp_remaining"] / iterations_per_matchup

            # Store in overall stats
            school_overall_stats[school1.name]["matchups"][school2.name] = stats["win_rate"]

            print(f"Win Rate: {stats['win_rate']:.1f}%")

    # Calculate overall win rates
    for school_name in school_overall_stats:
        stats = school_overall_stats[school_name]
        total_games = stats["total_wins"] + stats["total_losses"]
        if total_games > 0:
            stats["win_rate"] = (stats["total_wins"] / total_games) * 100

    return dict(matchup_stats), dict(school_overall_stats), schools

def generate_balance_report(matchup_stats, school_stats, schools):
    """Generate comprehensive balance report"""
    report_lines = []
    report_lines.append("="*80)
    report_lines.append("VYSTIA MAGE BALANCE REPORT")
    report_lines.append(f"Generated: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    report_lines.append("="*80)
    report_lines.append("")

    # Overall School Rankings
    report_lines.append("OVERALL SCHOOL RANKINGS (by Win Rate)")
    report_lines.append("-"*80)

    sorted_schools = sorted(school_stats.items(), key=lambda x: x[1]["win_rate"], reverse=True)

    for rank, (school_name, stats) in enumerate(sorted_schools, 1):
        tier = "S-Tier" if stats["win_rate"] >= 60 else \
               "A-Tier" if stats["win_rate"] >= 55 else \
               "B-Tier" if stats["win_rate"] >= 50 else \
               "C-Tier" if stats["win_rate"] >= 45 else \
               "D-Tier"

        report_lines.append(f"{rank}. {school_name:<20} - Win Rate: {stats['win_rate']:.1f}% "
                          f"({stats['total_wins']}W-{stats['total_losses']}L) [{tier}]")

    report_lines.append("")
    report_lines.append("="*80)
    report_lines.append("BALANCE ANALYSIS")
    report_lines.append("="*80)

    # Identify overpowered schools (>60% win rate)
    op_schools = [name for name, stats in sorted_schools if stats["win_rate"] > 60]
    if op_schools:
        report_lines.append("\n🔴 OVERPOWERED SCHOOLS (>60% win rate):")
        for school in op_schools:
            report_lines.append(f"  - {school}: {school_stats[school]['win_rate']:.1f}%")
            report_lines.append(f"    Recommendation: NERF damage or increase mana costs")

    # Identify underpowered schools (<40% win rate)
    up_schools = [name for name, stats in sorted_schools if stats["win_rate"] < 40]
    if up_schools:
        report_lines.append("\n🔵 UNDERPOWERED SCHOOLS (<40% win rate):")
        for school in up_schools:
            report_lines.append(f"  - {school}: {school_stats[school]['win_rate']:.1f}%")
            report_lines.append(f"    Recommendation: BUFF damage or reduce mana costs")

    # Identify balanced schools (45-55% win rate)
    balanced_schools = [name for name, stats in sorted_schools
                       if 45 <= stats["win_rate"] <= 55]
    if balanced_schools:
        report_lines.append("\n🟢 BALANCED SCHOOLS (45-55% win rate):")
        for school in balanced_schools:
            report_lines.append(f"  - {school}: {school_stats[school]['win_rate']:.1f}%")

    report_lines.append("")
    report_lines.append("="*80)
    report_lines.append("SPECIFIC MATCHUP ANALYSIS")
    report_lines.append("="*80)

    # Find problem matchups (>80% or <20% win rate)
    problem_matchups = []
    for matchup_name, stats in matchup_stats.items():
        if stats["win_rate"] > 80 or stats["win_rate"] < 20:
            problem_matchups.append((matchup_name, stats["win_rate"]))

    if problem_matchups:
        problem_matchups.sort(key=lambda x: abs(50 - x[1]), reverse=True)
        report_lines.append("\n⚠️  UNBALANCED MATCHUPS:")
        for matchup, win_rate in problem_matchups[:10]:
            report_lines.append(f"  {matchup}: {win_rate:.1f}%")

    report_lines.append("")
    report_lines.append("="*80)
    report_lines.append("DETAILED STATISTICS")
    report_lines.append("="*80)

    # Print win rate matrix
    report_lines.append("\nWIN RATE MATRIX (Row vs Column):")
    report_lines.append("-"*80)

    school_names = sorted(school_stats.keys())
    header = f"{'School':<18}"
    for name in school_names:
        header += f"{name[:6]:>7}"
    report_lines.append(header)
    report_lines.append("-"*80)

    for school1 in school_names:
        line = f"{school1:<18}"
        for school2 in school_names:
            if school1 == school2:
                line += f"{'---':>7}"
            else:
                win_rate = school_stats[school1]["matchups"].get(school2, 0)
                line += f"{win_rate:>6.1f}%"
        report_lines.append(line)

    # Save report
    report_text = "\n".join(report_lines)

    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    report_path = f"mage_balance_report_{timestamp}.txt"

    with open(report_path, 'w') as f:
        f.write(report_text)

    print("\n" + report_text)
    print(f"\nReport saved to: {report_path}")

    # Also save JSON data
    json_path = f"mage_balance_data_{timestamp}.json"
    with open(json_path, 'w') as f:
        json.dump({
            "matchup_stats": matchup_stats,
            "school_stats": school_stats,
            "timestamp": timestamp
        }, f, indent=2)

    print(f"Raw data saved to: {json_path}")

def main():
    parser = argparse.ArgumentParser(description="Vystia Mage Balance Tester")
    parser.add_argument("--iterations", type=int, default=100,
                       help="Number of iterations per matchup (default: 100)")

    args = parser.parse_args()

    # Run simulation
    matchup_stats, school_stats, schools = run_mass_simulation(args.iterations)

    # Generate report
    generate_balance_report(matchup_stats, school_stats, schools)

    print("\n" + "="*80)
    print("SIMULATION COMPLETE!")
    print("="*80)

if __name__ == "__main__":
    main()
