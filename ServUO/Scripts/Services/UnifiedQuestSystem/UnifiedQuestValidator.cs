using System;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Custom.VystiaClasses.Quests;
using Server.Engines.PartySystem;
using Server.Engines.Quests;

namespace Server.Services.UnifiedQuestSystem
{
    /// <summary>
    /// Unified quest validation system that consolidates all validation logic
    /// Replaces MultiplayerQuestValidator, QuestStateValidator, and QuestPlanValidator
    /// </summary>
    public static class UnifiedQuestValidator
    {
        private static readonly Dictionary<string, IQuestValidationStrategy> s_ValidationStrategies;
        private static readonly Dictionary<ValidationType, List<IQuestValidator>> s_Validators;
        private static readonly object s_Lock = new object();
        private static bool s_Initialized = false;
        private static int s_ValidationsPerformed = 0;
        private static int s_ValidationErrors = 0;

        static UnifiedQuestValidator()
        {
            s_ValidationStrategies = new Dictionary<string, IQuestValidationStrategy>();
            s_Validators = new Dictionary<ValidationType, List<IQuestValidator>>();
            
            // Initialize validation type collections
            foreach (ValidationType type in Enum.GetValues(typeof(ValidationType)))
            {
                s_Validators[type] = new List<IQuestValidator>();
            }
        }

        /// <summary>
        /// Initialize the unified quest validation system
        /// </summary>
        public static void Initialize()
        {
            if (s_Initialized)
                return;

            s_Initialized = true;

            // Register built-in validation strategies
            RegisterValidationStrategy("basic", new BasicValidationStrategy());
            RegisterValidationStrategy("multiplayer", new MultiplayerValidationStrategy());
            RegisterValidationStrategy("variety", new VarietyValidationStrategy());
            RegisterValidationStrategy("persistence", new PersistenceValidationStrategy());
            RegisterValidationStrategy("journal", new JournalValidationStrategy());

            // Register built-in validators
            RegisterValidator(ValidationType.Basic, new BasicQuestValidator());
            RegisterValidator(ValidationType.Multiplayer, new MultiplayerQuestValidator());
            RegisterValidator(ValidationType.Variety, new QuestVarietyValidator());
            RegisterValidator(ValidationType.Persistence, new QuestPersistenceValidator());
            RegisterValidator(ValidationType.Journal, new QuestJournalValidator());

            Console.WriteLine("[UnifiedQuestValidator] Initialized unified quest validation system");
            Console.WriteLine($"[UnifiedQuestValidator] Registered {s_ValidationStrategies.Count} validation strategies");
            Console.WriteLine($"[UnifiedQuestValidator] Registered {s_Validators.Values.Sum(list => list.Count)} validators");
        }

        /// <summary>
        /// Validate quest data using appropriate strategy
        /// </summary>
        public static ValidationResult ValidateQuest(UnifiedQuestData quest, ValidationContext context = null)
        {
            if (quest == null)
                return new ValidationResult { Result = ValidationStatus.Invalid, Issues = new List<ValidationIssue> { new ValidationIssue { Code = "NULL_QUEST", Description = "Quest data is null", Severity = "Critical" } } };

            lock (s_Lock)
            {
                try
                {
                    s_ValidationsPerformed++;

                    // Use default context if none provided
                    context = context ?? new ValidationContext();

                    // Select appropriate validation strategy
                    var strategy = SelectValidationStrategy(quest, context);
                    if (strategy == null)
                    {
                        return new ValidationResult { Result = ValidationStatus.Invalid, Issues = new List<ValidationIssue> { new ValidationIssue { Code = "NO_STRATEGY", Description = "No validation strategy available", Severity = "Critical" } } };
                    }

                    // Execute validation
                    var result = strategy.Validate(quest, context);

                    // Track errors
                    if (result.Result == ValidationStatus.Invalid || result.Result == ValidationStatus.Critical)
                    {
                        s_ValidationErrors++;
                    }

                    // Add metadata
                    result.ValidatedAt = DateTime.UtcNow;
                    result.ValidatorVersion = "1.0";
                    result.QuestId = quest.QuestId;
                    result.QuestType = quest.Type.ToString();

                    return result;
                }
                catch (Exception ex)
                {
                    s_ValidationErrors++;
                    Console.WriteLine($"[UnifiedQuestValidator] Error validating quest {quest.QuestId}: {ex.Message}");
                    
                    return new ValidationResult
                    {
                        Result = ValidationStatus.Critical,
                        Issues = new List<ValidationIssue>
                        {
                            new ValidationIssue
                            {
                                Code = "VALIDATION_EXCEPTION",
                                Description = $"Validation error: {ex.Message}",
                                Severity = "Critical",
                                Recommendation = "Check quest data integrity and try again"
                            }
                        },
                        ValidatedAt = DateTime.UtcNow,
                        ValidatorVersion = "1.0"
                    };
                }
            }
        }

        /// <summary>
        /// Validate quest with specific validation types
        /// </summary>
        public static ValidationResult ValidateQuestTypes(UnifiedQuestData quest, ValidationType[] validationTypes, ValidationContext context = null)
        {
            var combinedResult = new ValidationResult
            {
                Result = ValidationStatus.Valid,
                Issues = new List<ValidationIssue>(),
                ValidatedAt = DateTime.UtcNow,
                ValidatorVersion = "1.0"
            };

            foreach (var validationType in validationTypes)
            {
                var result = ValidateQuestType(quest, validationType, context);
                
                // Combine results
                combinedResult.Issues.AddRange(result.Issues);
                
                // Update overall result
                if (result.Result > combinedResult.Result)
                    combinedResult.Result = result.Result;
            }

            return combinedResult;
        }

        /// <summary>
        /// Validate quest with specific validation type
        /// </summary>
        public static ValidationResult ValidateQuestType(UnifiedQuestData quest, ValidationType validationType, ValidationContext context = null)
        {
            lock (s_Lock)
            {
                try
                {
                    s_ValidationsPerformed++;

                    var validators = s_Validators.GetValueOrDefault(validationType, new List<IQuestValidator>());
                    var combinedResult = new ValidationResult
                    {
                        Result = ValidationStatus.Valid,
                        Issues = new List<ValidationIssue>(),
                        ValidatedAt = DateTime.UtcNow,
                        ValidatorVersion = "1.0"
                    };

                    foreach (var validator in validators)
                    {
                        var result = validator.Validate(quest, context ?? new ValidationContext());
                        
                        // Combine results
                        combinedResult.Issues.AddRange(result.Issues);
                        
                        // Update overall result
                        if (result.Result > combinedResult.Result)
                            combinedResult.Result = result.Result;
                    }

                    // Track errors
                    if (combinedResult.Result == ValidationStatus.Invalid || combinedResult.Result == ValidationStatus.Critical)
                    {
                        s_ValidationErrors++;
                    }

                    return combinedResult;
                }
                catch (Exception ex)
                {
                    s_ValidationErrors++;
                    Console.WriteLine($"[UnifiedQuestValidator] Error validating quest {quest.QuestId} with type {validationType}: {ex.Message}");
                    
                    return new ValidationResult
                    {
                        Result = ValidationStatus.Critical,
                        Issues = new List<ValidationIssue>
                        {
                            new ValidationIssue
                            {
                                Code = "TYPE_VALIDATION_EXCEPTION",
                                Description = $"Validation error for {validationType}: {ex.Message}",
                                Severity = "Critical"
                            }
                        },
                        ValidatedAt = DateTime.UtcNow,
                        ValidatorVersion = "1.0"
                    };
                }
            }
        }

        /// <summary>
        /// Register a validation strategy
        /// </summary>
        public static void RegisterValidationStrategy(string name, IQuestValidationStrategy strategy)
        {
            lock (s_Lock)
            {
                s_ValidationStrategies[name] = strategy;
                Console.WriteLine($"[UnifiedQuestValidator] Registered validation strategy: {name}");
            }
        }

        /// <summary>
        /// Register a validator for specific validation type
        /// </summary>
        public static void RegisterValidator(ValidationType validationType, IQuestValidator validator)
        {
            lock (s_Lock)
            {
                s_Validators[validationType].Add(validator);
                Console.WriteLine($"[UnifiedQuestValidator] Registered validator for {validationType}");
            }
        }

        /// <summary>
        /// Get validation statistics
        /// </summary>
        public static ValidationStatistics GetStatistics()
        {
            lock (s_Lock)
            {
                return new ValidationStatistics
                {
                    ValidationsPerformed = s_ValidationsPerformed,
                    ValidationErrors = s_ValidationErrors,
                    SuccessRate = s_ValidationsPerformed > 0 ? (double)(s_ValidationsPerformed - s_ValidationErrors) / s_ValidationsPerformed : 0.0,
                    RegisteredStrategies = s_ValidationStrategies.Count,
                    RegisteredValidators = s_Validators.Values.Sum(list => list.Count),
                    LastValidation = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Reset validation statistics
        /// </summary>
        public static void ResetStatistics()
        {
            lock (s_Lock)
            {
                s_ValidationsPerformed = 0;
                s_ValidationErrors = 0;
                Console.WriteLine("[UnifiedQuestValidator] Statistics reset");
            }
        }

        /// <summary>
        /// Get available validation strategies
        /// </summary>
        public static List<string> GetAvailableStrategies()
        {
            lock (s_Lock)
            {
                return s_ValidationStrategies.Keys.ToList();
            }
        }

        /// <summary>
        /// Get validators for specific validation type
        /// </summary>
        public static List<IQuestValidator> GetValidators(ValidationType validationType)
        {
            lock (s_Lock)
            {
                return s_Validators.GetValueOrDefault(validationType, new List<IQuestValidator>()).ToList();
            }
        }

        /// <summary>
        /// Select appropriate validation strategy for quest
        /// </summary>
        private static IQuestValidationStrategy SelectValidationStrategy(UnifiedQuestData quest, ValidationContext context)
        {
            // Use specified strategy if provided
            if (!string.IsNullOrEmpty(context.StrategyName) && s_ValidationStrategies.TryGetValue(context.StrategyName, out var specifiedStrategy))
            {
                return specifiedStrategy;
            }

            // Auto-select strategy based on quest type
            switch (quest.Type)
            {
                case QuestType.Multiplayer:
                    return s_ValidationStrategies.GetValueOrDefault("multiplayer", s_ValidationStrategies.GetValueOrDefault("basic"));
                
                case QuestType.Dynamic:
                case QuestType.Vystia:
                    return s_ValidationStrategies.GetValueOrDefault("variety", s_ValidationStrategies.GetValueOrDefault("basic"));
                
                default:
                    return s_ValidationStrategies.GetValueOrDefault("basic");
            }
        }
    }

    /// <summary>
    /// Validation result
    /// </summary>
    public class ValidationResult
    {
        public ValidationStatus Result { get; set; }
        public List<ValidationIssue> Issues { get; set; }
        public DateTime ValidatedAt { get; set; }
        public string ValidatorVersion { get; set; }
        public int QuestId { get; set; }
        public string QuestType { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public ValidationResult()
        {
            Issues = new List<ValidationIssue>();
            Metadata = new Dictionary<string, object>();
            ValidatedAt = DateTime.UtcNow;
            ValidatorVersion = "1.0";
        }

        public bool IsValid => Result == ValidationStatus.Valid || Result == ValidationStatus.ValidWithWarnings;
        public bool HasWarnings => Result == ValidationStatus.ValidWithWarnings;
        public bool HasErrors => Result == ValidationStatus.Invalid || Result == ValidationStatus.Critical;
        public int IssueCount => Issues?.Count ?? 0;
        public int CriticalIssues => Issues?.Count(i => i.Severity == "Critical") ?? 0;
        public int WarningIssues => Issues?.Count(i => i.Severity == "Warning") ?? 0;
    }

    /// <summary>
    /// Validation context for validation operations
    /// </summary>
    public class ValidationContext
    {
        public string StrategyName { get; set; }
        public ValidationType[] ValidationTypes { get; set; }
        public PlayerMobile Validator { get; set; }
        public Party Party { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public ValidationSeverity MinimumSeverity { get; set; }
        public bool IncludeWarnings { get; set; }
        public bool IncludeRecommendations { get; set; }

        public ValidationContext()
        {
            Parameters = new Dictionary<string, object>();
            MinimumSeverity = ValidationSeverity.Info;
            IncludeWarnings = true;
            IncludeRecommendations = true;
        }
    }

    /// <summary>
    /// Validation statistics
    /// </summary>
    public class ValidationStatistics
    {
        public int ValidationsPerformed { get; set; }
        public int ValidationErrors { get; set; }
        public double SuccessRate { get; set; }
        public int RegisteredStrategies { get; set; }
        public int RegisteredValidators { get; set; }
        public DateTime LastValidation { get; set; }
    }

    /// <summary>
    /// Validation types
    /// </summary>
    public enum ValidationType
    {
        Basic,
        Multiplayer,
        Variety,
        Persistence,
        Journal,
        Performance,
        Security,
        Compatibility
    }

    /// <summary>
    /// Validation status
    /// </summary>
    public enum ValidationStatus
    {
        Valid,
        ValidWithWarnings,
        Invalid,
        Critical
    }

    /// <summary>
    /// Validation severity levels
    /// </summary>
    public enum ValidationSeverity
    {
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Interface for validation strategies
    /// </summary>
    public interface IQuestValidationStrategy
    {
        ValidationResult Validate(UnifiedQuestData quest, ValidationContext context);
        string StrategyName { get; }
        ValidationType[] SupportedTypes { get; }
    }

    /// <summary>
    /// Interface for quest validators
    /// </summary>
    public interface IQuestValidator
    {
        ValidationResult Validate(UnifiedQuestData quest, ValidationContext context);
        string ValidatorName { get; }
        ValidationType ValidationType { get; }
    }

    /// <summary>
    /// Extension methods for validation
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Validate quest and return result
        /// </summary>
        public static ValidationResult Validate(this UnifiedQuestData quest, ValidationContext context = null)
        {
            return UnifiedQuestValidator.ValidateQuest(quest, context);
        }

        /// <summary>
        /// Check if quest is valid
        /// </summary>
        public static bool IsValid(this UnifiedQuestData quest, ValidationContext context = null)
        {
            var result = UnifiedQuestValidator.ValidateQuest(quest, context);
            return result.IsValid;
        }

        /// <summary>
        /// Get validation issues for quest
        /// </summary>
        public static List<ValidationIssue> GetValidationIssues(this UnifiedQuestData quest, ValidationContext context = null)
        {
            var result = UnifiedQuestValidator.ValidateQuest(quest, context);
            return result.Isses ?? new List<ValidationIssue>();
        }

        /// <summary>
        /// Validate quest with specific types
        /// </summary>
        public static ValidationResult ValidateWithTypes(this UnifiedQuestData quest, params ValidationType[] validationTypes)
        {
            return UnifiedQuestValidator.ValidateQuestTypes(quest, validationTypes);
        }
    }
}
