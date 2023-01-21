<template>
  <div>
    <b-container>
      <b-card no-body>
        <b-tabs pills
                card
                vertical
                active-nav-item-class="active-nav">
          <b-tab v-if="books.length > 0" title="Books" active><b-card-text>
              <div v-for="book in books" :key="'book' + book.id">
                <SearchItem :item="book"
                            :id="book.id"
                            :img="book.coverPhotoPath"
                            root="/books/"
                            type="book">
                </SearchItem>
              </div>
          </b-card-text></b-tab>
          <b-tab v-if="magazines.length > 0" title="Magazines"><b-card-text>
            <div v-for="magazine in magazines" :key="'magazine' + magazine.id">
              <SearchItem :item="magazine"
                          :id="magazine.id"
                          :img="magazine.coverPhotoPath"
                          root="/magazines/"
                          type="magazine">
              </SearchItem>
            </div>
          </b-card-text></b-tab>
          <b-tab v-if="authors.length >0" title="Authors"><b-card-text>
            <div v-for="author in authors" :key="'author' + author.id">
              <SearchItem :item="author"
                          :img="author.photographPath"
                          :id="author.id"
                          root="/authors/"
                          type="author">
              </SearchItem>
            </div>
          </b-card-text></b-tab>
          <b-tab v-if="genres.length > 0" title="Genres"><b-card-text>
            <div v-for="genre in genres" :key="'genre' + genre.id">
              <SearchItem :item="genre"
                          :id="genre.id"
                          root="/genre/"
                          type="genre">
              </SearchItem>
            </div>
          </b-card-text></b-tab>
          <b-tab v-if="fields.length > 0" title="Fields"><b-card-text>
            <div v-for="field in fields" :key="'field' + field.id">
              <SearchItem :item="field"
                          :id="field.id"
                          root="/fields/"
                          type="field">
              </SearchItem>
            </div>
          </b-card-text></b-tab>
        </b-tabs>
      </b-card>
    </b-container>
  </div>
</template>

<script>
import SearchItem from "@/components/search_page/SearchItem";

export default {
  name: 'SearchResults',
  components: {
    SearchItem
  },
  props: {
    books: [],
    authors: [],
    magazines: [],
    genres: [],
    fields: [],
    root: String
  }
}
</script>

<style>
  @import "../../assets/styles/main.css";
  @import "../../assets/styles/search.css";
</style>