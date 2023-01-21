# [KNN - Recommendation system]
### About
Solution: *KNN - Semestral project - Recommendation system*
Authors: *Jan Lorenc, Marek Hlavačka, Jaromír Wysoglad*
University: *Brno University of technology: Faculty of information technology*
Year: *2021/22*

The solution implements hybrid collaborative/content-based recommendation system which is further described in the technical report /doc/technical_report.pdf.

### Implementation
In /src directory can be found 3 files:
* `data_processing.ipynb` - Downloads and processes datasets. Available are "Whole Goodreads dataset", "Poetry" (used for development due to small size), "Comics and Graphic" (goal dataset). In addition and under the presumption that all books are downloaded and processed as it filters from them, goodbooks-10k dataset is downloaded and processed (used for comparison and evaluation).
* `content_embeddings.ipynb` - Trains content-based book embeddings which are further used in final hybrid model.
* `model.ipynb` - Implements final model. There can be set 3 architectures (baseline MF, NCF, hybrid) to train.

All notebooks should be able to be run on any environment supporting Jupyter notebooks. Basic presumption is Collab and paths are set for our drive, so this will have to be changed for your machine/account. Most of `model.ipynb` development was done on Paperspace Gradient platform and an environment switch variable between collab/paperspace is added in the notebook. However, this only changes data paths and mounting G-drive, so data paths are truly the only thing to be handled for it to run anywhere.

### Data samples
In /data directory can be found 2 samples of input data to the final hybrid model:
* `book_interactions_comics_graphic_sample.csv` - Contains several interactions consisting of user_id, book_id and rating.
* `book_embedding_comics_graphic_64_sample.csv` - Contains few pretrained content-based embeddings for books.

### Results
In /doc/results are included printed notebooks with all 6 results mentioned in the technical report:
* `MF-comics-result.pdf`
* `NCF-comics-result.pdf`
* `hybrid-comics-result.pdf`
* `MF-kaggle-result.pdf`
* `NCF-kaggle-result.pdf`
* `hybrid-kaggle-result.pdf`
