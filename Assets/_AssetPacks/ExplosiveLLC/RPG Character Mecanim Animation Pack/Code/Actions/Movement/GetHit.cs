// Hit from front1 - 1
// Hit from front2 - 2
// Hit from back - 3
// Hit from left - 4
// Hit from right - 5
// Block Break1 - 6

using RPGCharacterAnims.Extensions;
using RPGCharacterAnims.Lookups;

namespace RPGCharacterAnims.Actions
{
    public class GetHit : MovementActionHandler<HitContext>
    {
        public GetHit(RPGCharacterMovementController movement) : base(movement)
        {
        }

        public override bool CanStartAction(RPGCharacterController controller)
        { return !controller.isKnockback && !controller.isKnockdown && !controller.isSpecial; }

        protected override void _StartAction(RPGCharacterController controller, HitContext context)
        {
            var hitNumber = context.number;
            var direction = context.direction;
            var force = context.force;
            var variableForce = context.variableForce;

            if (hitNumber == -1) {
                if (controller.isBlocking) {
                    hitNumber = (int)AnimationVariations.BlockedHits.TakeRandom();
                    direction = AnimationData.HitDirection((BlockedHitType)hitNumber);
                    force = 3f;
                    variableForce = 3f;
                }
				else {
                    hitNumber = (int)AnimationVariations.Hits.TakeRandom();
                    direction = AnimationData.HitDirection((HitType)hitNumber);
                }
                direction = controller.transform.rotation * direction;
            }
			else {
                if (context.relative) { direction = controller.transform.rotation * direction; }
				if (hitNumber == 6) { hitNumber = 3; }
            }

            controller.GetHit(hitNumber);
            movement.KnockbackForce(direction, force, variableForce);
        }
    }
}