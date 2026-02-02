# GpxAnimator

GpxAnimator ist ein C#-Tool zur Animation und Visualisierung von GPX-Tracks. Es ermöglicht das Einlesen von GPX-Dateien, das Rendern von Animationsframes und das Exportieren als Video.

## Features
- Einlesen und Verarbeiten von GPX-Dateien
- Animierte Visualisierung von GPS-Tracks
- Unterstützung für verschiedene Animationsarten (z.B. Pfad, Höhe, Textfade)
- Export als Video (über Encoder)


## Voraussetzungen
- .NET 10.0 SDK oder höher

## Nutzung
1. GPX-Dateien im Ordner `gpx_files` ablegen.
2. Projekt bauen:
	```
	dotnet build
	```
3. Animation starten:
	```
	dotnet run --project GpxAnimator
	```

## Lizenz
Siehe LICENSE.txt
