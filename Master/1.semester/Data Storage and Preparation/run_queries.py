#!/usr/bin/env python3

# File: run_queries.py
# Solution: UPA - project
# Date: 9.10.2021
# Authors: Jan Lorenc, Marek Hlavačka, Martin Smetana
# Faculty: Faculty of information technology VUT
# Description: Runs all queries from 2. part of the project.


from src.query_A_extractor import QueryAExtractor
from src.query_A_visualiser import QueryAVisualiser
from src.query_B_extractor import QueryBExtractor
from src.query_B_visualiser import QueryBVisualiser
from src.query_C_extractor import QueryCExtractor
from src.query_C_processor import QueryCProcessor
from src.query_custom_extractor import QueryCustomExtractor
from src.query_custom_visualiser import QueryCustomVisualiser


if __name__ == "__main__":
    QueryAExtractor.run()
    QueryAVisualiser.run()
    QueryBExtractor.run()
    QueryBVisualiser.run()
    QueryCExtractor.run()
    QueryCProcessor.run()
    QueryCustomExtractor.run()
    QueryCustomVisualiser.run()
