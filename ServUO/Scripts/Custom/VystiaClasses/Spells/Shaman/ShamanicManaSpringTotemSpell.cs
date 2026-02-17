using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.Shamanic
{
    /// <summary>
    /// Mana Spring Totem - Mana Spring Totem
    /// Circle: 4 (16 mana)
    /// </summary>
    public class ShamanicManaSpringTotemSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Mana Spring Totem", "Manaum Springum Totemum",
            203,
            9041,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Shamanic;

        public ShamanicManaSpringTotemSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;            // Vystia Reagent Check
            if (!Caster.Backpack.ConsumeTotal(typeof(StormEssence), 1) || !Caster.Backpack.ConsumeTotal(typeof(SpiritFeather), 1))
            {
                Caster.SendMessage("You do not have enough reagents to cast this spell.");
                Caster.SendMessage("Required: StormEssence (1), SpiritFeather (1)");
                return false;
            }



            return true;
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                // Visual effect at caster
                Caster.FixedParticles(0x3728, 10, 15, 5038, 0x21, 0, EffectLayer.Waist);
                Caster.PlaySound(0x212);

                // Mana Spring Totem - Mana Spring Totem Circle: 4 (16 mana)
                // Create totem item at caster's location
                Point3D loc = Caster.Location;
                Map map = Caster.Map;

                // Totem effect (simplified - aura buff)
                double duration = 1.0 + (Caster.Skills.Magery.Value / 60.0);

                Caster.SendMessage(0x3B2, "You plant a totem!");
                Caster.SendMessage(0x22, "(Totem effect active for " + duration.ToString("F1") + " minutes)");

                // Apply totem buff to nearby allies
                IPooledEnumerable eable = map.GetMobilesInRange(loc, 8);

                foreach (Mobile m in eable)
                {
                    if (m == Caster || (m is PlayerMobile && m.Party == Caster.Party))
                    {
                        m.AddStatMod(new StatMod(StatType.Str, "ShamanicManaSpringTotem_Totem", 10, TimeSpan.FromMinutes(duration)));
                        m.FixedParticles(0x375A, 10, 15, 5013, 0x21, 0, EffectLayer.Waist);
                    }
                }
                eable.Free();
            }

            FinishSequence();
        }
    }
}
