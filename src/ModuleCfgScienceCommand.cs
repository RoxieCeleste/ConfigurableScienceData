using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConfigurableScienceData.src {
	class ModuleCfgScienceCommand : ModuleScienceContainer {
		new public void ReviewDataEvent() {
			ReviewData();
		}

		new public void ReviewData() {
			foreach(ScienceData data in GetData()) {
				ReviewDataItem(data);
			}
		}

		new public void ReviewDataItem(ScienceData data) {
			ExperimentResultDialogPage page = new ExperimentResultDialogPage(
				part,
				data,
				data.transmitValue,
				ModuleCfgScienceLab.GetBoostForVesselData(part.vessel, data),
				false,
				"",
				false,
				data.labBoost < 1 && vessel.FindPartModulesImplementing<ModuleCfgScienceLab>().Count > 0 && ModuleScienceLab.IsLabData(data),
				new Callback<ScienceData>(onDiscardData),
				new Callback<ScienceData>(onKeepData),
				new Callback<ScienceData>(onTransmitData),
				new Callback<ScienceData>(onSendDataToLab));
			ExperimentsResultDialog.DisplayResult(page);
		}

		/* Experiment Result Dialog Page Callbacks */
		public void onDiscardData(ScienceData data) {
			DumpData(data);
		}

		public void onKeepData(ScienceData data) {
		}

		public void onTransmitData(ScienceData data) {
			List<IScienceDataTransmitter> transList = vessel.FindPartModulesImplementing<IScienceDataTransmitter>();
			if(transList.Count > 0) {
				IScienceDataTransmitter transmitter = transList.First(t => t.CanTransmit());
				if(transmitter != null) {
					transmitter.TransmitData(new List<ScienceData> { data });
					DumpData(data);
				}
				else {
					ScreenMessages.PostScreenMessage("No opperational transmitter on this vessel.", 4f, ScreenMessageStyle.UPPER_CENTER);
				}
			}
			else {
				ScreenMessages.PostScreenMessage("No transmitter on this vessel.", 4f, ScreenMessageStyle.UPPER_CENTER);
			}
		}

		public void onSendDataToLab(ScienceData data) {
			List<ModuleCfgScienceLab> labList = vessel.FindPartModulesImplementing<ModuleCfgScienceLab>();
			if(labList.Count > 0) {
				ModuleCfgScienceLab lab = labList.FirstOrDefault(l => l.IsOperational());
				if(lab != null) {
					lab.StartCoroutine(labList.FirstOrDefault().ProcessData((CfgScienceData)data, new Callback<ScienceData>(onLabComplete)));
				}
				else {
					ScreenMessages.PostScreenMessage("No opperational science lab on this vessel.", 4f, ScreenMessageStyle.UPPER_CENTER);
				}
			}
			else {
				ScreenMessages.PostScreenMessage("No science lab on this vessel.", 4f, ScreenMessageStyle.UPPER_CENTER);
			}
		}

		public void onLabComplete(ScienceData data) {
			ReviewDataItem(data);
		}
	}
}
