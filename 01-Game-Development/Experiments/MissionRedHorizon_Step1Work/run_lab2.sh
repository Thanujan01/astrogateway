#!/bin/zsh
W=/Users/kapilash/Projects/MissionRedHorizon_Step1Work
U=/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity
mkdir -p $W/results/Final/Step1 $W/results/Final/Step2
STEP1_OUT=$W/results/Final/Step1 $U -batchmode -projectPath $W/Lab2 -executeMethod AstroGateway.Verification.Step1GameplayVerification.Run -logFile $W/results/Final/Step1/Unity.log
echo "STEP1 EXIT $?"
STEP2_OUT=$W/results/Final/Step2 $U -batchmode -projectPath $W/Lab2 -executeMethod AstroGateway.MissionIntegration.MissionIntegrationBuilder.Validate -logFile $W/results/Final/Step2/Unity.log
echo "STEP2 EXIT $?"
sed -n 2p $W/results/Final/Step1/Step1_Gameplay_Results.txt
sed -n 2p $W/results/Final/Step2/Mission_Integration_Results.txt
grep -hE '^FAIL|EXCEPTION|WARN' $W/results/Final/Step1/Step1_Gameplay_Results.txt $W/results/Final/Step2/Mission_Integration_Results.txt
echo ALLDONE
