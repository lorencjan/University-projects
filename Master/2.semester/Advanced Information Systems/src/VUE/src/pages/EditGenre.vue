<template>
  <div>
    <NavbarFinal></NavbarFinal>
    <b-container class="edit-book-page">
      <b-form @submit.prevent="submit">
        <b-row>
          <b-col>
            <b-form-group
                id="genre_name-label"
                label="Genre name:"
                label-class="required"
                label-for="genre_name"
            >
              <b-form-input
                  ref="genre_name"
                  id="genre_name"
                  v-model="genre.name"
                  type="text"
                  placeholder="Enter genre name"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Genre name can not be empty!
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row v-if="this.$route.params.id != 0">
          <b-col class="text-center mt-4">
            <b-button @click="submit" variant="primary">Update</b-button>
          </b-col>
        </b-row>
        <b-row v-if="this.$route.params.id == 0">
          <b-col class="text-center mt-4">
            <b-button @click="create" variant="success">Create</b-button>
          </b-col>
        </b-row>
      </b-form>
    </b-container>
  </div>
</template>
<script>

import ApiConnect from "@/services/ApiConnect";
import Multiselect from "vue-multiselect";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: "EditBook",
  components: {
    Multiselect,
    NavbarFinal
  },
  data () {
    return {
      selectedBooks: [],
      books: [],
      isLoading: false,
      genres: [],
      isLoadingGenre: false,
      genre: {},
    }
  },
  methods: {
    limitTextBook (count) {
      return `and ${count} other Books`
    },
    asyncFindBook(query) {
      this.isLoading = true
      let params = {params: {"name": query}};
      ApiConnect.get('/books/', params).then(response => {
        console.log(response.data,query)
        this.books = response.data
        this.isLoading = false
      })

    },
    clearAll () {
      this.selectedBooks = []
    },
    getGenre(id){
      ApiConnect.get('/genres/'+id).then((response) =>{
        this.genre = response.data
      });
    },
    getBooks(){
      let params = {params: {"genres": this.genre.name}};
      ApiConnect.get('/books/',params).then((response) =>{
        this.selectedBooks = response.data
      })
    },
    submit(){
      if (! this.genre.name){
        this.$refs['genre_name'].state = false;
        this.$refs['genre_name'].value = "";
        return;
      }
        ApiConnect.put('/genres', this.genre).then((response) =>{
          this.$refs['genre_name'].state = null;
          this.makeToast('Genre  '+this.genre.name+' has been updated successfully.')
        }).catch(error => {
          console.log(error)
        })
    },
    create(){
      if (! this.genre.name){
        this.$refs['genre_name'].state = false;
        this.$refs['genre_name'].value = "";
        return;
      }
        ApiConnect.post('/genres', this.genre).then((response) =>{
          this.makeToast('Genre '+this.genre.name+' has been created successfully.')
          console.log(response)
        }).catch(error => {
          console.log(error)
        })

      ApiConnect.get('/genres/').then(resp =>{
        this.$router.push('/edit_genres/'+(resp.data[resp.data.length -1].id+1))
      })

    },
    makeToast(text) {
      this.$bvToast.toast(text, {
        title: 'Library',
        variant: 'success',
        autoHideDelay: 5000,
      })
    }
  },
  created() {
    if(this.$route.params.id == 0){
      this.genre = {
            id: 0,
            name: '',
      }
    }else{
      this.getGenre(this.$route.params.id);
    }
    this.getBooks();
  },
  computed: {
    hasElectronicCopy (){
      if (this.book !== 'undefined'){
        if (this.book.electronicCopyExemplars.length > 0	){
          return true;
        }
      }

      return false;
    }
  }
}
</script>
<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>
<style scoped>
.edit-book-page{
  color: black;
  text-align: left;
}
.preview{
  color: #24433e;
  box-shadow: 0 6px 10px rgba(0,0,0,0), 0 0 6px rgba(0,0,0,0);
  transition: .3s transform cubic-bezier(.155,1.105,.295,1.12),.3s box-shadow,.3s -webkit-transform cubic-bezier(.155,1.105,.295,1.12);
}
.preview:hover{
  transform: scale(1.05);
  box-shadow: 0 10px 20px rgba(0,0,0,.12), 0 4px 8px rgba(0,0,0,.06);
}
</style>