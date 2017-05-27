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
	public interface IValidationError {
		object Object {
			get;
		}

		Type ObjectType {
			get;
		}

		FieldInfo FieldInfo {
			get;
		}
	}
}
