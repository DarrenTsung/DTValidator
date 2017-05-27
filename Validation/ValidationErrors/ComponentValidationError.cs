using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

using DTValidator.Internal;

namespace DTValidator {
	public class ComponentValidationError : IValidationError {
		// PRAGMA MARK - Public Interface
		public readonly Component Component;
		public readonly Type ComponentType;
		public readonly FieldInfo FieldInfo;

		public ComponentValidationError(Component component, Type componentType, FieldInfo fieldInfo) {
			Component = component;
			ComponentType = componentType;
			FieldInfo = fieldInfo;
		}


		// PRAGMA MARK - IValidationError Implementation
		object IValidationError.Object {
			get { return Component; }
		}

		Type IValidationError.ObjectType {
			get { return ComponentType; }
		}

		FieldInfo IValidationError.FieldInfo {
			get { return FieldInfo; }
		}
	}
}
