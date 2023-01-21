<template>
  <div>
    <NavBar  @event="search_input= $event" @deleteSearch="deleteSearchInput"></NavBar>
    <div v-if="search_input">
      <SearchResults v-if="filteredBooks.length > 0 ||
                            filteredMagazines.length > 0 ||
                            filteredAuthors.length > 0 ||
                            filteredGenres.length > 0 ||
                            filteredFields.length > 0"
                     :books="filteredBooks"
                     :magazines="filteredMagazines"
                     :authors="filteredAuthors"
                     :genres="filteredGenres"
                     :fields="filteredFields">
      </SearchResults>
      <b-container v-else>
        <b-alert class="mt-2" show variant="danger" dismissible> No search result </b-alert>
      </b-container>
  </div>
    <div v-else>

    </div>
  </div>
</template>

<script>
import NavBar from "@/components/main_page/NavBar";
import SearchResults from "@/components/search_page/SearchResults";

import ApiConnect from "@/services/ApiConnect";

export default {
  name: "NavbarFinal",
  components: {
    NavBar,
    SearchResults
  },
  data(){
    return {
      search_input: '',
      books: [],
      magazines: [],
      authors: [],
      genres: [],
      fields: []
    }
  },
  methods: {
    getBooks(){
      ApiConnect.get('books').then((response) =>
          this.books = response.data,
      )},
    getMagazines(){
      ApiConnect.get('magazines/').then((response) =>
          this.magazines = response.data,
      )},
    getAuthors(){
      ApiConnect.get('authors/').then((response) => this.authors = response.data, )
    },
    getFields(){
      ApiConnect.get('fields/').then((response) => this.fields = response.data, )
    },
    getGenres(){
      ApiConnect.get('genres/').then((response) => this.genres = response.data, )
    },
    deleteSearchInput(val){
      this.search_input = '';
    },
    hideContent(){
      this.$emit('hideContent');
    },
  },
  created() {
    this.getBooks();
    this.getMagazines();
    this.getAuthors();
    this.getGenres();
    this.getFields();
  },
  computed: {
    filteredBooks : function (){
      return this.books.filter(book =>
          book.name.toLowerCase().includes(this.search_input.toLowerCase()));

    },
    filteredMagazines : function (){
      return this.magazines.filter(magazine =>
          magazine.name.toLowerCase().includes(this.search_input.toLocaleLowerCase()));
    },
    filteredAuthors : function (){
      return this.authors.filter(author =>
          author.name.toLowerCase().includes(this.search_input.toLowerCase()) ||
          author.surname.toLowerCase().includes(this.search_input.toLowerCase())
      );
    },
    filteredGenres: function(){
      return this.genres.filter(genre =>
          genre.name.toLowerCase().includes(this.search_input.toLowerCase())
      );
    },
    filteredFields: function(){
      return this.fields.filter(field =>
          field.name.toLowerCase().includes(this.search_input.toLowerCase())
      );
    }
  }
}
</script>

<style>
  @import "../../assets/styles/main.css";
</style>