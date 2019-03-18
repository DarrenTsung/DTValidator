using System.Collections.Generic;
using UnityEngine;

namespace DTValidator.TestBlacklisted
{
    public class BlacklistedOutletComponent : MonoBehaviour
    {
        public GameObject Outlet;
    }
}

namespace DTValidator.Internal
{
    using DTValidator.TestBlacklisted;
    using NUnit.Framework;

    public static class ValidatorBlacklistedClassTests
    {
        private static IList<ValidatorBlacklistedClass> BlacklistedOutletComponentClassProvider()
        {
            var blacklistedClass = ScriptableObject.CreateInstance<ValidatorBlacklistedClass>();
            blacklistedClass.Class = "DTValidator.TestIgnore";

            return new ValidatorBlacklistedClass[] { blacklistedClass };
        }

        // [Test]
        public static void BlacklistedMissingOutlet_ReturnsNoErrors()
        {
            ValidatorBlacklistedClassProvider.SetCurrentProvider(BlacklistedOutletComponentClassProvider);
            ValidatorBlacklistedClassProvider.SetCurrentProvider(() => new ValidatorBlacklistedClass[0]);

            GameObject gameObject = new GameObject();

            var outletComponent = gameObject.AddComponent<BlacklistedOutletComponent>();
            outletComponent.Outlet = null;

            IList<IValidationError> errors = Validator.Validate(gameObject);
            Assert.That(errors, Is.Null);

            ValidatorIgnoredNamespaceProvider.ClearCurrentProvider();
            ValidatorWhitelistedNamespaceProvider.ClearCurrentProvider();
            ValidatorBlacklistedClassProvider.ClearCurrentProvider();
        }
    }
}
