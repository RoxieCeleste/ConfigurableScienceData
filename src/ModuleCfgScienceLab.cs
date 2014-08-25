using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConfigurableScienceData {
	public class ModuleCfgScienceLab : ModuleScienceLab {

		public override void OnStart(StartState state) {
			base.OnStart(state);
		}

		public override void OnLoad(ConfigNode node) {
			base.OnLoad(node);
		}

		public override void OnSave(ConfigNode node) {
			base.OnSave(node);
		}

		new public static float GetBoostForVesselData(Vessel host, ScienceData data){//CfgScienceData data) {
			float result = 0f;

			if(data is CfgScienceData){
				if(((CfgScienceData)data).boostType.Equals(CfgScienceData.BoostType.TransmitValue)) {
					Debug.Log("[SC] Xmit Value");
					result = ((CfgScienceData)data).boostValue * data.transmitValue;
				}
				else if(((CfgScienceData)data).boostType.Equals(CfgScienceData.BoostType.BaseValue)) {
					Debug.Log("[SC] Base Value");
					result = ((CfgScienceData)data).boostValue;
				}
				else if(((CfgScienceData)data).boostType.Equals(CfgScienceData.BoostType.FlatValue)) {
					Debug.Log("[SC] Flat Value");
					result = ((CfgScienceData)data).boostValue / data.dataAmount;
				}
			}
			else{
				result = ModuleScienceLab.GetBoostForVesselData(host, data);
			}
			return result;
		}

		public IEnumerator ProcessData(CfgScienceData data, Callback<ScienceData> onComplete) {
			if(data.boostType.Equals(CfgScienceData.BoostType.TransmitValue)) {
				data.transmitValue = (data.transmitValue * (1f + data.boostValue)) / 1.5f;
			}
			else if(data.boostType.Equals(CfgScienceData.BoostType.BaseValue)) {
				data.transmitValue = (data.transmitValue + data.boostValue) / 1.5f;
			}
			else if(data.boostType.Equals(CfgScienceData.BoostType.BaseValue)) {
				data.transmitValue = (data.transmitValue * data.dataAmount + data.boostValue) / (1.5f * data.dataAmount);
			}

			if(data.transmitValue * 1.5 > 1) {
				data.transmitValue = (1f / 1.5f);
			}

			IEnumerator result = base.ProcessData(data, onComplete);
			return result;
		}

		public float getBoostForData(CfgScienceData data) {
			return data.boostValue;
		}

		public ModuleScienceLab getLab() {
			return part.FindModuleImplementing<ModuleScienceLab>();
		}

		new public void ProcessScienceDataEvent() {
			List<IScienceDataContainer> containers = vessel.FindPartModulesImplementing<IScienceDataContainer>().Where(c => c.GetScienceCount() > 0).ToList();
			foreach(IScienceDataContainer c in containers) {
				ScienceData[] data = c.GetData();
				foreach(ScienceData d in data) {
					if(IsLabData(d)){
						c.ReviewDataItem(d);
					}
				}
			}
		}
	}
}