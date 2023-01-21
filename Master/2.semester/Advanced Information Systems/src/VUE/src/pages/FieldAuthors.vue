<template>
  <div>
    <NavbarFinal></NavbarFinal>
    <b-container>
      <h1>Authors:</h1>
      <b-row>
        <AuthorTile v-for="author in authors"
                    :img="author.photographPath"
                    :name = author.name
                    :surname = author.surname
                    :id = author.id
                    :dateOfBirth ="new Date(author.dateOfBirth)"
        >

        </AuthorTile>
      </b-row>
    </b-container>
  </div>
</template>

<script>
import AuthorTile from "@/components/genre_page/AuthorTile";
import ApiConnect from "@/services/ApiConnect";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: "FieldAuthors",
  components: {AuthorTile, NavbarFinal},
  data(){
    return {
      books: [],
      field_name: [],
      authors: [],
    }
  },

  methods: {
    getMagazines(name){
      let params = {params:{"fields": name}};
      ApiConnect.get('magazines/',params).then((response) =>
          {
            this.books = response.data;
            this.books.forEach(book => this.authors.push(book.authors[0]))
          }
      )},

    getName(){
      let id = this.$route.params.id
      if (typeof(this.$route.params.id) == 'undefined'){
        id = ''
      }
      ApiConnect.get('fields/'+id).then((response) =>
          {
            this.field_name = response.data;
            this.getMagazines(this.field_name.name);
          }
      )},


  },
  created() {
    this.getName();
  },


}
</script>

<style scoped>

</style>