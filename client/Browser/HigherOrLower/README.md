# Higher or Lower

This sample is a simple in-browser game that uses the nether JavaScript SDK.

The instructions below show how to run this sample and the nether API locally.

Before proceding, you should clone the [nether](https://github.com/MicrosoftDX/nether) repo and ensure that you are able to run that successfully.

## Running the sample (PowerShell)

Open a PowerShell window and run `scripts\setnetherconfig.ps1`. This sets environment variables to configure nether to allow the sample to connect.
In the same window, launch nether via the `rundev.ps1` script.


Now that you have nether running, create a new PowerShell window and run `scripts\rungame.ps1` to launch the game.

You can now launch your browser and navigate to `http://localhost:8080` to test the sample.


## Running the sample (bash)

Open a bash window and source the `scripts\setnetherconfig.ps1`

```bash
    $ source path-to-sample/scripts/setnetherconfig.ps1
```

This sets the environment variables to configure nether to allow the sample to connect.
In the same window, launch nether via the `rundev.sh` script.


Now that you have nether running, create a new PowerShell window and run `scripts\rungame.sh` to launch the game.

You can now launch your browser and navigate to `http://localhost:8080` to test the sample.
