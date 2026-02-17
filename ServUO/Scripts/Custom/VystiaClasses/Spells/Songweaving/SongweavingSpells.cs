#if VYSTIA_SONGWEAVING
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Custom.VystiaClasses.Songweaving;
using Server.Custom.VystiaClasses.Systems;

namespace Server.Spells.VystiaSpells.Songweaving
{
    public abstract class SongweavingSpellBase : Spell
    {
        protected SongweavingSpellBase(Mobile caster, Item scroll, SpellInfo info)
            : base(caster, scroll, info)
        {
        }

        public override SkillName CastSkill => SkillName.Songweaving;
        public override SkillName DamageSkill => SkillName.Songweaving;
        public override bool ClearHandsOnCast => false;
        public override bool ShowHandMovement => false;
        public override bool CheckNextSpellTime => false;
        public override bool RevealOnCast => true;

        public override int GetMana() => 0;
        public override TimeSpan CastDelayBase => TimeSpan.Zero;

        public override bool CheckFizzle() => true;

        public override bool CheckCast()
        {
            if (!(Caster is PlayerMobile pm))
            {
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            if (Caster is PlayerMobile pm)
            {
                Perform(pm);
            }

            FinishSequence();
        }

        protected abstract void Perform(PlayerMobile caster);
    }

    public abstract class SongweavingSongSpellBase : SongweavingSpellBase
    {
        private readonly string m_SongKey;

        protected SongweavingSongSpellBase(Mobile caster, Item scroll, SpellInfo info, string songKey)
            : base(caster, scroll, info)
        {
            m_SongKey = songKey;
        }

        protected override void Perform(PlayerMobile caster)
        {
            ISongweavingSong song = SongweavingRegistry.GetByKey(m_SongKey);
            if (song == null)
            {
                caster.SendMessage("That song is not available.");
                return;
            }

            song.Begin(caster);
        }
    }

    public abstract class SongweavingFinaleSpellBase : SongweavingSpellBase
    {
        private readonly string m_FinaleKey;

        protected SongweavingFinaleSpellBase(Mobile caster, Item scroll, SpellInfo info, string finaleKey)
            : base(caster, scroll, info)
        {
            m_FinaleKey = finaleKey;
        }

        protected override void Perform(PlayerMobile caster)
        {
            FinaleDefinition finale = SongweavingFinaleSystem.GetByKey(m_FinaleKey);
            if (finale == null)
            {
                caster.SendMessage("That finale is not available.");
                return;
            }

            SongweavingFinaleSystem.BeginFinale(caster, finale);
        }
    }

    public class SongweavingProvocationSpell : SongweavingSongSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Song of Provocation", "Vox Provocare", 16);
        public SongweavingProvocationSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "provocation") { }
    }

    public class SongweavingPeacemakingSpell : SongweavingSongSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Lullaby", "Somnus", 16);
        public SongweavingPeacemakingSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "peacemaking") { }
    }

    public class SongweavingDiscordanceSpell : SongweavingSongSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Discordant Note", "Discordia", 16);
        public SongweavingDiscordanceSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "discordance") { }
    }

    public class SongweavingRequiemSpell : SongweavingSongSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Dirge of Weakness", "Requiem", 16);
        public SongweavingRequiemSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "requiem") { }
    }

    public class SongweavingMendingSpell : SongweavingSongSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Song of Healing", "Sanare", 16);
        public SongweavingMendingSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "mending") { }
    }

    public class SongweavingCourageSpell : SongweavingSongSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Song of Courage", "Valor", 16);
        public SongweavingCourageSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "courage") { }
    }

    public class SongweavingSwiftnessSpell : SongweavingSongSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Song of Swiftness", "Celeritas", 16);
        public SongweavingSwiftnessSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "swiftness") { }
    }

    public class SongweavingLightSpell : SongweavingSongSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Song of Illumination", "Lux", 16);
        public SongweavingLightSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "light") { }
    }

    public class SongweavingFortuneSpell : SongweavingSongSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Inspire Accuracy", "Accurus", 16);
        public SongweavingFortuneSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "fortune") { }
    }

    public class SongweavingSharpNoteSpell : SongweavingFinaleSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Sharp Note", "Acuta", 16);
        public SongweavingSharpNoteSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "sharpnote") { }
    }

    public class SongweavingInterludeSpell : SongweavingFinaleSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Mesmerise", "Mesmerum", 16);
        public SongweavingInterludeSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "interlude") { }
    }

    public class SongweavingRallyingAnthemSpell : SongweavingFinaleSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Cacophony", "Cacophonia", 16);
        public SongweavingRallyingAnthemSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "rally") { }
    }

    public class SongweavingFortissimoSpell : SongweavingFinaleSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Fortissimo", "Fortissimus", 16);
        public SongweavingFortissimoSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "fortissimo") { }
    }

    public class SongweavingSoothingChorusSpell : SongweavingFinaleSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Soothing Chorus", "Lenis", 16);
        public SongweavingSoothingChorusSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "soothing") { }
    }

    public class SongweavingSymphonyOfDestructionSpell : SongweavingFinaleSpellBase
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Symphony of Destruction", "Ruinus", 16);
        public SongweavingSymphonyOfDestructionSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info, "symphony") { }
    }
}
#endif
