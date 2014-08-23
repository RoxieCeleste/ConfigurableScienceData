using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConfigurableScienceData {
	public class CfgScienceData : ScienceData {
		public enum BoostType {
			BaseValue,
			TransmitValue,
			FlatValue
		}

		public float boostValue = .5f;

		public BoostType boostType = BoostType.TransmitValue;

		public CfgScienceData(float baseValue, float xmitValue, float labBoost, string id, string dataName, float boostValue, BoostType boostType)
			: base(baseValue, xmitValue, labBoost, id, dataName) {
			this.boostValue = boostValue;
			this.boostType = boostType;
		}

		public CfgScienceData(ScienceData data)
			: this(data.dataAmount, data.transmitValue, data.labBoost, data.subjectID, data.title, .5f, BoostType.TransmitValue) {
		}

		public CfgScienceData(ScienceData data, float boostValue, BoostType boostType)
			: this(data.dataAmount, data.transmitValue, data.labBoost, data.subjectID, data.title, boostValue, boostType) {
		}

		public CfgScienceData(float baseValue, float xmitValue, float boostValue, BoostType boostType, string id, string dataName)
			: this(baseValue, xmitValue, 0f, id, dataName, boostValue, boostType) {

		}

		public static CfgScienceData CopyOf(CfgScienceData data) {
			return new CfgScienceData(data.dataAmount, data.transmitValue, data.labBoost, data.subjectID, data.title, data.boostValue, data.boostType);
		}

		public CfgScienceData(ConfigNode node)
			: base(node) {
			this.Load(node);
		}

		new public void Load(ConfigNode node) {
			if(node.HasValue("boostValue")) {
				this.boostValue = float.Parse(node.GetValue("boostValue"));
			}
			if(node.HasValue("boostType")) {
				this.boostType = (BoostType)Enum.Parse(typeof(BoostType), node.GetValue("boostType"));
			}
		}

		new public void Save(ConfigNode node) {
			base.Save(node);
			node.AddValue("boostValue", boostValue);
			node.AddValue("boostType", boostType);
		}
	}
}