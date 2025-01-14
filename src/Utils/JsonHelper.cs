﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable MemberCanBePrivate.Global

namespace ProjectGenesis.Utils
{
    internal static class JsonHelper
    {
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        internal static ItemProtoJson[] ItemProtos()
            => JsonConvert
               .DeserializeObject<ItemProtoJson[]>(new StreamReader(Assembly.GetManifestResourceStream("ProjectGenesis.dependencies.items.json"))
                                                      .ReadToEnd());

        internal static RecipeProtoJson[] RecipeProtos()
            => JsonConvert
               .DeserializeObject<RecipeProtoJson[]>(new StreamReader(Assembly.GetManifestResourceStream("ProjectGenesis.dependencies.recipes.json"))
                                                        .ReadToEnd());

        internal static TechProtoJson[] TechProtos()
            => JsonConvert
               .DeserializeObject<TechProtoJson[]>(new StreamReader(Assembly.GetManifestResourceStream("ProjectGenesis.dependencies.techs.json"))
                                                      .ReadToEnd());

        internal static StringProtoJson[] StringProtos()
            => JsonConvert
               .DeserializeObject<StringProtoJson[]>(new StreamReader(Assembly.GetManifestResourceStream("ProjectGenesis.dependencies.strings.json"))
                                                        .ReadToEnd());

        internal static string SerializeObject(object obj)
            => JsonConvert.SerializeObject(obj, Formatting.Indented,
                                           new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        internal static void ExportAsJson(string path)
        {
            File.WriteAllText($"{path}/techs.json", SerializeObject(LDB.techs.dataArray.Select(TechProtoJson.FromProto)));
            File.WriteAllText($"{path}/items.json", SerializeObject(LDB.items.dataArray.Select(ItemProtoJson.FromProto)));
            File.WriteAllText($"{path}/recipes.json", SerializeObject(LDB.recipes.dataArray.Select(RecipeProtoJson.FromProto)));
            File.WriteAllText($"{path}/strings_all.json", SerializeObject(LDB.strings.dataArray.Select(StringProtoJson.FromProto)));
        }

        [Serializable]
        public class ItemProtoJson
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string IconPath { get; set; }
            public int GridIndex { get; set; }
            public int StackSize { get; set; }
            public int Type { get; set; }
            public int PreTech { get; set; }
            public int PreTechOverride { get; set; }
            public int[] DescFields { get; set; }
            public int FuelType { get; set; }
            public long HeatValue { get; set; }
            public float ReactorInc { get; set; }
            public bool IsFluid { get; set; }
            public bool Productive { get; set; }
            public int SubID { get; set; }
            public string MiningFrom { get; set; }
            public string ProduceFrom { get; set; }
            public int Grade { get; set; }
            public int[] Upgrades { get; set; }
            public bool IsEntity { get; set; }
            public bool CanBuild { get; set; }
            public bool BuildInGas { get; set; }
            public int ModelIndex { get; set; }
            public int ModelCount { get; set; }
            public int HpMax { get; set; }
            public int Ability { get; set; }
            public long Potential { get; set; }
            public int BuildIndex { get; set; }
            public int BuildMode { get; set; }
            public int UnlockKey { get; set; }
            public int MechaMaterialID { get; set; }

            public static ItemProtoJson FromProto(ItemProto i)
                => new ItemProtoJson
                   {
                       ID = i.ID,
                       Name = i.Name,
                       Description = i.Description,
                       IconPath = i.IconPath,
                       GridIndex = i.GridIndex,
                       StackSize = i.StackSize,
                       FuelType = i.FuelType,
                       HeatValue = i.HeatValue,
                       ReactorInc = i.ReactorInc,
                       DescFields = i.DescFields,
                       IsFluid = i.IsFluid,
                       PreTech = i.preTech?.ID ?? i.PreTechOverride,
                       Type = (int)i.Type,
                       SubID = i.SubID,
                       MiningFrom = i.MiningFrom,
                       ProduceFrom = i.ProduceFrom,
                       Grade = i.Grade,
                       Upgrades = i.Upgrades,
                       IsEntity = i.IsEntity,
                       CanBuild = i.CanBuild,
                       BuildInGas = i.BuildInGas,
                       ModelIndex = i.ModelIndex,
                       ModelCount = i.ModelCount,
                       HpMax = i.HpMax,
                       Ability = i.Ability,
                       Potential = i.Potential,
                       BuildIndex = i.BuildIndex,
                       BuildMode = i.BuildMode,
                       UnlockKey = i.UnlockKey,
                       PreTechOverride = i.PreTechOverride,
                       Productive = i.Productive,
                       MechaMaterialID = i.MechaMaterialID
                   };
        }

        [Serializable]
        public class RecipeProtoJson
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string IconPath { get; set; }
            public int Type { get; set; }
            public int GridIndex { get; set; }
            public int Time { get; set; }
            public int[] Input { get; set; }
            public int[] InCounts { get; set; }
            public int[] Output { get; set; }
            public int[] OutCounts { get; set; }
            public bool Explicit { get; set; }
            public bool Handcraft { get; set; }
            public bool NonProductive { get; set; }

            public static RecipeProtoJson FromProto(RecipeProto i)
                => new RecipeProtoJson
                   {
                       ID = i.ID,
                       Explicit = i.Explicit,
                       Name = i.Name,
                       Handcraft = i.Handcraft,
                       Type = (int)i.Type,
                       Time = i.TimeSpend,
                       Input = i.Items ?? Array.Empty<int>(),
                       InCounts = i.ItemCounts ?? Array.Empty<int>(),
                       Output = i.Results ?? Array.Empty<int>(),
                       OutCounts = i.ResultCounts ?? Array.Empty<int>(),
                       Description = i.Description,
                       GridIndex = i.GridIndex,
                       IconPath = i.IconPath,
                       NonProductive = i.NonProductive
                   };
        }

        [Serializable]
        public class TechProtoJson
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string IconPath { get; set; }
            public string Desc { get; set; }
            public string Conclusion { get; set; }
            public float[] Position { get; set; }
            public int[] PreTechs { get; set; }
            public int[] PreTechsImplicit { get; set; }
            public int[] Items { get; set; }
            public int[] ItemPoints { get; set; }
            public long HashNeeded { get; set; }
            public int[] UnlockRecipes { get; set; }
            public int[] UnlockFunctions { get; set; }
            public double[] UnlockValues { get; set; }
            public bool Published { get; set; }
            public int Level { get; set; }
            public int MaxLevel { get; set; }
            public int LevelCoef1 { get; set; }
            public int LevelCoef2 { get; set; }
            public bool IsLabTech { get; set; }
            public bool PreTechsMax { get; set; }
            public int[] AddItems { get; set; }
            public int[] AddItemCounts { get; set; }
            public int[] PropertyOverrideItems { get; set; }
            public int[] PropertyItemCounts { get; set; }

            public static TechProtoJson FromProto(TechProto i)
                => new TechProtoJson
                   {
                       ID = i.ID,
                       Name = i.Name,
                       Desc = i.Desc,
                       Conclusion = i.Conclusion,
                       Published = i.Published,
                       Level = i.Level,
                       MaxLevel = i.MaxLevel,
                       LevelCoef1 = i.LevelCoef1,
                       LevelCoef2 = i.LevelCoef2,
                       IconPath = i.IconPath,
                       IsLabTech = i.IsLabTech,
                       PreTechs = i.PreTechs,
                       PreTechsImplicit = i.PreTechsImplicit,
                       PreTechsMax = i.PreTechsMax,
                       Items = i.Items,
                       ItemPoints = i.ItemPoints,
                       HashNeeded = i.HashNeeded,
                       UnlockRecipes = i.UnlockRecipes.Distinct().ToArray(),
                       UnlockFunctions = i.UnlockFunctions,
                       UnlockValues = i.UnlockValues,
                       AddItems = i.AddItems,
                       AddItemCounts = i.AddItemCounts,
                       Position = new[] { i.Position.x, i.Position.y },
                       PropertyOverrideItems = i.PropertyOverrideItems,
                       PropertyItemCounts = i.PropertyItemCounts
                   };
        }
    }

    [Serializable]
    public class StringProtoJson
    {
        public string Name { get; set; }
        public string ZHCN { get; set; }
        public string ENUS { get; set; }
        public static StringProtoJson FromProto(StringProto i) => new StringProtoJson { Name = i.Name, ZHCN = i.ZHCN, ENUS = i.ENUS };
    }
}
