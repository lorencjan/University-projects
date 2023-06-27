.PHONY: monitor logs bad

monitor: cart_log.txt
	./cart_monitor.py <$<

bad:
	./cart_monitor.py <cart_bad_log.txt

logs: cart_log.txt cart_bad_log.txt

cart_log.txt: requests.csv
	./cartctl_test.py $< >$@

cart_bad_log.txt: requests-bad.csv
	./cartctl_test.py $< >$@
