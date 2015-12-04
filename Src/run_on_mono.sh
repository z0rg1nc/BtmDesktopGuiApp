#!/bin/bash
export MONO_THREADS_PER_CPU=2000
chmod +x ./Updater/run_on_mono.sh
mono ./BitMoneyClient.exe --use-mono=1 $@