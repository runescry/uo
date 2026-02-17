using System;
using System.Collections.Generic;
using Server;

namespace Server.Services.LLM
{
    /// <summary>
    /// Random name generator for LLM NPCs
    /// </summary>
    public static class NPCNames
    {
        private static readonly string[] MaleFirstNames = new string[]
        {
            "Aldric", "Baldric", "Cedric", "Darian", "Eldrin", "Fenris", "Gareth", "Hadrian",
            "Ignatius", "Jasper", "Kael", "Lucius", "Magnus", "Nolan", "Osric", "Percival",
            "Quinlan", "Roderick", "Sebastian", "Thaddeus", "Ulric", "Victor", "Wolfric", "Xavier",
            "Yorick", "Zephyr", "Alaric", "Benedict", "Cornelius", "Dominic", "Edmund", "Felix",
            "Gregory", "Henrik", "Ivan", "Julius", "Klaus", "Leopold", "Marcus", "Nathaniel",
            "Oliver", "Phineas", "Quentin", "Reginald", "Silas", "Tobias", "Uther", "Vincent",
            "Wilhelm", "Xander", "Zachary", "Adrian", "Bernard", "Cyrus", "Duncan", "Everett",
            "Francis", "Gideon", "Hugo", "Isaac", "Jerome", "Kieran", "Linus", "Matthias"
        };

        private static readonly string[] FemaleFirstNames = new string[]
        {
            "Adriana", "Beatrice", "Celestia", "Diana", "Elara", "Fiona", "Gwyneth", "Helena",
            "Isolde", "Juliana", "Katarina", "Lydia", "Morgana", "Natalia", "Ophelia", "Petra",
            "Quintessa", "Rosalind", "Seraphina", "Tatiana", "Ursula", "Vivienne", "Willow", "Xenia",
            "Yvonne", "Zara", "Anastasia", "Bianca", "Cassandra", "Delilah", "Elena", "Freya",
            "Giselle", "Harriet", "Iris", "Jasmine", "Kira", "Lilith", "Meredith", "Nora",
            "Octavia", "Penelope", "Quinn", "Rowena", "Sylvia", "Thalia", "Uma", "Valentina",
            "Winifred", "Xiomara", "Yasmin", "Zoe", "Arabella", "Bridget", "Clarissa", "Daphne",
            "Evangeline", "Felicity", "Genevieve", "Isadora", "Josephine", "Katerina", "Lucinda", "Margot"
        };

        private static readonly string[] LastNames = new string[]
        {
            "Ashwood", "Blackwell", "Cromwell", "Darkwater", "Emberforge", "Frostwind", "Goldsmith",
            "Hawthorne", "Ironside", "Lockwood", "Moonwhisper", "Nightshade", "Oakenheart", "Ravenswood",
            "Silverbrook", "Stormcaller", "Thornfield", "Windrunner", "Ashford", "Brightblade",
            "Copperfield", "Drakemore", "Evergreen", "Fairweather", "Grimstone", "Highwater", "Ironwood",
            "Kingsley", "Lightfoot", "Millbrook", "Northwind", "Proudfoot", "Quicksilver", "Redmane",
            "Shadowmere", "Thistlewood", "Underhill", "Valorheart", "Whitestone", "Youngblood",
            "Bronzehammer", "Crystalbrook", "Dawnbringer", "Flameheart", "Goldleaf", "Stonefist",
            "Swiftarrow", "Winterborn", "Starlight", "Ironheart", "Stonebridge", "Willowmere",
            "Foxglove", "Brightwood", "Darkholme", "Fireborn", "Icevein", "Thunderstrike"
        };

        /// <summary>
        /// Generates a random full name based on gender
        /// </summary>
        public static string GetRandomName(bool isFemale)
        {
            string firstName = isFemale
                ? FemaleFirstNames[Utility.Random(FemaleFirstNames.Length)]
                : MaleFirstNames[Utility.Random(MaleFirstNames.Length)];

            string lastName = LastNames[Utility.Random(LastNames.Length)];

            return $"{firstName} {lastName}";
        }

        /// <summary>
        /// Generates a random name with a title/profession
        /// </summary>
        public static string GetRandomNameWithTitle(bool isFemale, string title)
        {
            string name = GetRandomName(isFemale);
            return $"{name} {title}";
        }
    }
}
