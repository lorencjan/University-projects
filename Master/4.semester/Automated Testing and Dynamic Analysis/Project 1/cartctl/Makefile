test:
	python3 cartctl_test.py -b

verb:
	python3 cartctl_test.py -v

.SUFFIXES: .png .gv

.gv.png:
	dot -Tpng -o $@ $<

jarvis.gv: jarvisenv.py
	python -c 'import jarvisenv; jarvisenv.JARVIS_TRACKS.export("jarvis.gv")'
