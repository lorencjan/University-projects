<template>
  <div class="book-list">
      <h1 class="book-list-title">{{type}}</h1>
      <b-container class="book-user-list">
        <b-card style="background-color: transparent; border: none"  no-body>
          <b-card-body
              id="nav-scroller"
              ref="content"
              style="position:relative;
                     height:330px;
                     overflow-y:scroll;
                     padding-top: 0px;
                     padding-bottom: 0px;">

            <div v-for="val in sortedData">
              <div v-if="electronic">
                <div v-if="val.electronicCopy.book != undefined">
                  <div v-if="borrowing">
                    <BookListItem :name="val.electronicCopy.book.name"
                                  type="book"
                                  :id="val.id"
                                  :user="user"
                                  :data="val"
                                  :borrowing="true"
                                  :electronic="true"
                                  :book = "val.electronicCopy.book"
                                  :file="val.electronicCopy.id"
                                  :img="val.electronicCopy.book.coverPhotoPath"
                                  :dateFrom="new Date(val.dateOfBorrowStart)"
                                  :dateTo="new Date(val.dateOfBorrowEnd)"
                                  :state="val.state"></BookListItem>
                  </div>
                  <div v-else>
                    <BookListItem :name="val.electronicCopy.book.name"
                                  type="book"
                                  :book="val.electronicCopy.book"
                                  :id="val.id"
                                  :user="user"
                                  :data="val"
                                  :borrowing="false"
                                  :electronic="true"
                                  :file="val.electronicCopy.id"
                                  :img="val.electronicCopy.book.coverPhotoPath"
                                  :dateFrom="new Date(val.dateFrom)"
                                  :dateTo="new Date(val.dateUntil)"
                                  :state="val.state"></BookListItem>
                  </div>

                </div>
                <div v-if="val.electronicCopy.magazine != undefined">
                  <div v-if="borrowing">
                    <BookListItem :name="val.electronicCopy.magazine.name"
                                  type="magazine"
                                  :id="val.id"
                                  :magazine="val.electronicCopy.magazine"
                                  :user="user"
                                  :data="val"
                                  :file="val.electronicCopy.id"
                                  :borrowing="true"
                                  :electronic="true"
                                  :img="val.electronicCopy.magazine.coverPhotoPath"
                                  :dateFrom="new Date(val.dateOfBorrowStart)"
                                  :dateTo="new Date(val.dateOfBorrowEnd)"
                                  :state="val.state"></BookListItem>
                  </div>
                  <div v-else>
                    <BookListItem :name="val.electronicCopy.magazine.name"
                                  type="magazine"
                                  :magazine="val.electronicCopy.magazine"
                                  :id="val.id"
                                  :user="user"
                                  :data="val"
                                  :borrowing="false"
                                  :electronic="true"
                                  :file="val.electronicCopy.id"
                                  :img="val.electronicCopy.magazine.coverPhotoPath"
                                  :dateFrom="new Date(val.dateFrom)"
                                  :dateTo="new Date(val.dateUntil)"
                                  :state="val.state"></BookListItem>
                  </div>
                </div>

              </div>
              <div v-if="!electronic">
              <div v-if="val.exemplar.book != undefined">
                <div v-if="borrowing">
                  <BookListItem :name="val.exemplar.book.name"
                                type="book"
                                :id="val.id"
                                :book="val.exemplar.book"
                                :user="user"
                                :data="val"
                                :borrowing="true"
                                :img="val.exemplar.book.coverPhotoPath"
                                :dateFrom="new Date(val.dateOfBorrowStart)"
                                :dateTo="new Date(val.dateOfBorrowEnd)"
                                :state="val.state"></BookListItem>
                </div>
                <div v-else>
                  <BookListItem :name="val.exemplar.book.name"
                                type="book"
                                :book="val.exemplar.book"
                                :id="val.id"
                                :user="user"
                                :data="val"
                                :borrowing="false"
                                :img="val.exemplar.book.coverPhotoPath"
                                :dateFrom="new Date(val.dateFrom)"
                                :dateTo="new Date(val.dateUntil)"
                                :state="val.state"></BookListItem>
                </div>

              </div>
              <div v-if="val.exemplar.magazine != undefined">
                <div v-if="borrowing">
                  <BookListItem :name="val.exemplar.magazine.name"
                                type="magazine"
                                :id="val.id"
                                :magazine="val.exemplar.magazine"
                                :user="user"
                                :data="val"
                                :borrowing="true"
                                :img="val.exemplar.magazine.coverPhotoPath"
                                :dateFrom="new Date(val.dateOfBorrowStart)"
                                :dateTo="new Date(val.dateOfBorrowEnd)"
                                :state="val.state"></BookListItem>
                </div>
                <div v-else>
                  <BookListItem :name="val.exemplar.magazine.name"
                                type="magazine"
                                :magazine="val.exemplar.magazine"
                                :id="val.id"
                                :user="user"
                                :data="val"
                                :borrowing="false"
                                :img="val.exemplar.magazine.coverPhotoPath"
                                :dateFrom="new Date(val.dateFrom)"
                                :dateTo="new Date(val.dateUntil)"
                                :state="val.state"></BookListItem>
                </div>
              </div>
              </div>
            </div>

          </b-card-body>
        </b-card>
      </b-container>
  </div>
</template>

<script>
  import BookListItem from "@/components/user_page/BookListItem";
  import ApiConnect from "@/services/ApiConnect";
  export default {
    name: 'BookList',
    props: {
      type: String,
      data: {},
      borrowing: Boolean,
      electronic: Boolean,
      user: {},
    },
    components: {
      BookListItem
    },
    computed: {
      sortedData: function() {
        if (this.borrowing){
          this.data.sort( (a,b) => {
            return new Date(b.dateOfBorrowEnd) - new Date(a.dateOfBorrowEnd);
          });
        }
        else {
          this.data.sort( (a,b) => {
            return new Date(b.dateUntil) - new Date(a.dateUntil);
          });
        }

        return this.data;
      },
    }

  }
</script>