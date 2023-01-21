<template>
  <div id="books">
    <NavbarFinal></NavbarFinal>
    <b-container>
      <h1><span v-if="this.$route.query.genres !== undefined">{{ this.$route.query.genres }}</span>  Books:</h1>
      <b-row>
        <MainTile v-for="book in books"
                  :img="book.coverPhotoPath"
                  :genres = book.genres
                  :type = book.language
                  :name = book.name
                  :authors = book.authors
                  :fields = book.fields
                  :id = book.id
                  root = "/books/"
        >

        </MainTile>
      </b-row>
    </b-container>
  </div>
</template>

<script>
import MainTile from "@/components/main_page/MainTile";
import NavbarFinal from "@/components/main_page/NavbarFinal";
import ApiConnect from "@/services/ApiConnect";


export default {
  name: "Books",
  components: {
    MainTile,
    NavbarFinal
  },
  data(){
    return {
      books: [],
    }
  },
  methods: {
    getBooks(){
      let params;
      if(this.$route.query.genres !== undefined) {
        params = {params: {"genres": this.$route.query.genres}}
      }else{
        params = {params: {}}
      }
      ApiConnect.get('/books', params).then((response) =>
          this.books = response.data,
      )}
  },
  created() {
    this.getBooks();
      console.log(this.$route.query.genre);
  },
}
</script>

<style scoped>

</style>