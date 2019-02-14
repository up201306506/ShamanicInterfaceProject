using UnityEngine;
using System.Collections;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Models.Markov.Topology;
using Accord.Statistics.Distributions.Fitting;
using ShamanicInterface.DataStructure;
using ShamanicInterface.Utils;
using System.IO;
public class NewCreateModelsScript : MonoBehaviour {

	
	private static string directory_name = "GestureModels";

	void Start()
	{
		Debug.Log("Starting:");
		System.IO.Directory.CreateDirectory(directory_name);
				
		for(int i = 2; i <= 20; i += 2) {
			CreateModelFromFrames("Frames/LEFT_NEW.frs", directory_name+"/LEFT_NEW"+i.ToString()+".bin", i);
		}
		for(int i = 2; i <= 20; i += 2) {
			CreateModelFromFrames("Frames/RIGHT_NEW.frs", directory_name+"/RIGHT_NEW"+i.ToString()+".bin", i);
		}
		File.AppendAllText(directory_name+"/LOGS.txt", "-----------\n\n");
	}

	public static void CreateModelFromFrames(string readPath, string writePath, int forwNum) {
		SequenceList seq = Utils.FramesToSequenceList(Utils.LoadListListFrame(readPath));

		HiddenMarkovModel<MultivariateNormalDistribution> hmm;
		MultivariateNormalDistribution mnd = new MultivariateNormalDistribution(seq.GetArray()[0][0].Length);
		hmm = new HiddenMarkovModel<MultivariateNormalDistribution>(new Forward(forwNum), mnd);

		var teacher = new BaumWelchLearning<MultivariateNormalDistribution>(hmm);
		teacher.Tolerance = 0.0001;
		teacher.Iterations = 0;
		teacher.FittingOptions = new NormalOptions()
		{
			Diagonal = true,      // only diagonal covariance matrices
			Regularization = 1e-5 // avoid non-positive definite errors
		};
		
		double logLikelihood = teacher.Run(seq.GetArray());
		
		Debug.Log(writePath + ":" + forwNum.ToString() + " - " + seq.sequences.Count + " - " + logLikelihood);
		File.AppendAllText(directory_name+"/LOGS.txt", writePath + ":" + forwNum.ToString() + " - " + seq.sequences.Count + " - " + logLikelihood + "\n");
		
		hmm.Save(writePath);
	}

}
