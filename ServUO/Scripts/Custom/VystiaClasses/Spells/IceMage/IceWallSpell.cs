using System;
using Server;
using Server.Targeting;
using Server.Spells;
using Server.Items;

using Server.Spells.VystiaSpells;
namespace Server.Spells.VystiaSpells.IceMage
{
    /// <summary>
    /// Ice Wall - Creates a barrier of ice
    /// Creates a 5-tile long wall that blocks movement
    /// Circle: 4th (11 mana)
    /// Reagents: Glacier Crystal, Permafrost Essence (Vystia reagents)
    /// </summary>
    public class IceWallSpell : VystiaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Ice Wall", "Kal Frio Murus",
            236,
            9011,
            false
        );

        public override SpellCircle Circle => SpellCircle.Fourth;
        public override VystiaMagicSchool MagicSchool => VystiaMagicSchool.Ice;

        public IceWallSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (!HasReagents(typeof(GlacierCrystal), 1) || !HasReagents(typeof(PermafrostEssence), 1))
            {
                Caster.SendMessage(0x22, "You do not have the required reagents (Glacier Crystal, Permafrost Essence).");
                return false;
            }

            return true;
        }

        public override bool ConsumeReagents()
        {
            return ConsumeReagent(typeof(GlacierCrystal), 1) && ConsumeReagent(typeof(PermafrostEssence), 1);
        }

        private bool HasReagents(Type type, int amount)
        {
            return (Caster.Backpack != null && Caster.Backpack.GetAmount(type) >= amount);
        }

        private bool ConsumeReagent(Type type, int amount)
        {
            if (Caster.Backpack == null)
                return false;

            return Caster.Backpack.ConsumeTotal(type, amount);
        }

        public override void OnCast()
        {
// Check fizzle and trigger skill gain

            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237);
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                if (p is Item)
                    p = ((Item)p).GetWorldLocation();

                Point3D loc = new Point3D(p);

                // Create a 5x5 square of ice walls
                Effects.PlaySound(loc, Caster.Map, 0x1F8);

                for (int x = -2; x <= 2; x++)
                {
                    for (int y = -2; y <= 2; y++)
                {
                        Point3D wallLoc = new Point3D(loc.X + x, loc.Y + y, loc.Z);
                        
                        // Determine if wall should be horizontal or vertical based on position
                        bool horizontal = (Math.Abs(x) > Math.Abs(y)); // More horizontal than vertical

                    IceWallItem wall = new IceWallItem(Caster, horizontal);
                    wall.MoveToWorld(wallLoc, Caster.Map);

                    Effects.SendLocationParticles(
                        EffectItem.Create(wallLoc, Caster.Map, EffectItem.DefaultDuration),
                        0x376A, 9, 10, 0x481, 0, 5052, 0);
                    }
                }

                Caster.SendMessage(0x3B2, "You create a wall of ice!");
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private IceWallSpell m_Owner;

            public InternalTarget(IceWallSpell owner) : base(12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;
                if (p != null)
                    m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }

    /// <summary>
    /// Ice Wall Item - Physical barrier
    /// </summary>
    public class IceWallItem : Item
    {
        private int m_HP;
        private Mobile m_Caster;
        private Timer m_ExpireTimer;

        public IceWallItem(Mobile caster, bool horizontal) : base(horizontal ? 0x0080 : 0x007E)
        {
            m_Caster = caster;
            m_HP = 100;
            Movable = false;
            Hue = 0x481;
            Name = "ice wall";

            // Auto-expire after 30 seconds
            m_ExpireTimer = Timer.DelayCall(TimeSpan.FromSeconds(30.0), delegate
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(Location, Map, EffectItem.DefaultDuration),
                    0x3735, 1, 30, 0x481, 0, 5052, 0);
                Delete();
            });
        }

        public IceWallItem(Serial serial) : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_ExpireTimer != null && m_ExpireTimer.Running)
                m_ExpireTimer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_HP);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_HP = reader.ReadInt();

            // Restart expiration timer on server restart
            m_ExpireTimer = Timer.DelayCall(TimeSpan.FromSeconds(30.0), Delete);
        }
    }
}
