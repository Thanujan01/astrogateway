#!/bin/zsh
# Runs the earlier sessions' regression suites on the lab copy. Usage: run_regression.sh <label>
W=/Users/kapilash/Projects/MissionRedHorizon_Step1Work
U=/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity
R=$W/results/Regression_$1
mkdir -p $R
for pair in ${=SUITES:-HUD:CinematicHUDBuilder.Validate Responsive:HUDResponsiveQA.Responsive Native:NativeSliceBuilder.ValidateExisting Mars:Stage08BBuilder.BatchValidate Mission01:Mission01Builder.ValidateExisting VisualPolish:MarsResearchBaseBuilder.ValidateExisting}; do
  name=${pair%%:*}; method=${pair#*:}
  $U -batchmode -projectPath $W/Lab -executeMethod $method -logFile $R/${name}_Unity.log
  echo "$name exit $?" | tee -a $R/Execution.txt
done
rm -rf $R/Suites; cp -R $W/results/Regression $R/Suites
echo DONE
