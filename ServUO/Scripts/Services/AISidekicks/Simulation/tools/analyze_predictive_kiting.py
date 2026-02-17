#!/usr/bin/env python3
"""
Analyze Predictive Kiting Results

Analyzes results from predictive_kiting_optimizer.py to identify best parameters.
"""

import json
import sys
from typing import Dict, List
from collections import defaultdict

def analyze_results(results_file: str) -> Dict:
    """Analyze simulation results and generate report"""
    with open(results_file, 'r') as f:
        data = json.load(f)
    
    results = data['results']
    
    # Sort by victory rate
    sorted_results = sorted(results, key=lambda x: x['summary']['victory_rate'], reverse=True)
    
    # Top 10 parameter sets
    top_10 = sorted_results[:10]
    
    # Parameter correlation analysis
    param_correlations = defaultdict(list)
    
    for result in results:
        victory_rate = result['summary']['victory_rate']
        for param_name, param_value in result['parameters'].items():
            param_correlations[param_name].append((param_value, victory_rate))
    
    # Calculate average victory rate for each parameter value range
    param_analysis = {}
    for param_name, correlations in param_correlations.items():
        # Group by value ranges
        ranges = {}
        for value, victory_rate in correlations:
            # Create range buckets
            if isinstance(value, float):
                bucket = round(value, 1)
            else:
                bucket = value
            
            if bucket not in ranges:
                ranges[bucket] = []
            ranges[bucket].append(victory_rate)
        
        # Calculate average victory rate per range
        avg_by_range = {}
        for bucket, rates in ranges.items():
            avg_by_range[bucket] = sum(rates) / len(rates)
        
        # Find best range
        best_bucket = max(avg_by_range.items(), key=lambda x: x[1])
        param_analysis[param_name] = {
            'best_value': best_bucket[0],
            'best_avg_victory_rate': best_bucket[1],
            'all_ranges': avg_by_range
        }
    
    # Generate report
    report = {
        'total_parameter_sets': len(results),
        'top_10_parameter_sets': top_10,
        'parameter_analysis': param_analysis,
        'overall_stats': {
            'avg_victory_rate': sum(r['summary']['victory_rate'] for r in results) / len(results),
            'max_victory_rate': max(r['summary']['victory_rate'] for r in results),
            'min_victory_rate': min(r['summary']['victory_rate'] for r in results),
        }
    }
    
    return report


def print_report(report: Dict):
    """Print analysis report"""
    print("=" * 80)
    print("Predictive Kiting Analysis Report")
    print("=" * 80)
    
    print(f"\nOverall Statistics:")
    print(f"  Total Parameter Sets: {report['total_parameter_sets']}")
    print(f"  Average Victory Rate: {report['overall_stats']['avg_victory_rate']:.2%}")
    print(f"  Max Victory Rate: {report['overall_stats']['max_victory_rate']:.2%}")
    print(f"  Min Victory Rate: {report['overall_stats']['min_victory_rate']:.2%}")
    
    print(f"\n{'=' * 80}")
    print("Top 10 Parameter Sets by Victory Rate:")
    print("=" * 80)
    
    for i, result in enumerate(report['top_10_parameter_sets'], 1):
        print(f"\n{i}. Parameter Set {result['parameter_set']}")
        print(f"   Victory Rate: {result['summary']['victory_rate']:.2%}")
        print(f"   Death Rate: {result['summary']['death_rate']:.2%}")
        print(f"   Avg Survival: {result['summary']['avg_survival_ticks']:.1f} ticks")
        print(f"   Avg Distance: {result['summary']['avg_distance']:.1f} tiles")
        print(f"   Avg Spells: {result['summary']['avg_spells_cast']:.1f}")
        print(f"   Parameters:")
        for key, value in sorted(result['parameters'].items()):
            print(f"     {key}: {value}")
    
    print(f"\n{'=' * 80}")
    print("Parameter Sensitivity Analysis:")
    print("=" * 80)
    
    for param_name, analysis in sorted(report['parameter_analysis'].items()):
        print(f"\n{param_name}:")
        print(f"  Best Value: {analysis['best_value']}")
        print(f"  Best Avg Victory Rate: {analysis['best_avg_victory_rate']:.2%}")
        print(f"  Value Range Analysis:")
        for value, avg_rate in sorted(analysis['all_ranges'].items()):
            print(f"    {value}: {avg_rate:.2%}")


def main():
    """Main entry point"""
    import argparse
    
    parser = argparse.ArgumentParser(description='Analyze predictive kiting optimization results')
    parser.add_argument('--input', type=str, default='predictive_kiting_results.json', help='Input results file')
    parser.add_argument('--output', type=str, default='predictive_kiting_analysis.json', help='Output analysis file')
    
    args = parser.parse_args()
    
    print(f"Analyzing results from {args.input}...")
    report = analyze_results(args.input)
    
    # Save analysis
    with open(args.output, 'w') as f:
        json.dump(report, f, indent=2)
    
    print(f"\nAnalysis saved to {args.output}")
    
    # Print report
    print_report(report)


if __name__ == '__main__':
    main()

