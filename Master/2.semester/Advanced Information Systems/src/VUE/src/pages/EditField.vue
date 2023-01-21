<template>
  <div>
    <NavbarFinal></NavbarFinal>
    <b-container class="edit-book-page">
      <b-form @submit.prevent="submit">
        <b-row>
          <b-col>
            <b-form-group
                id="field_name-label"
                label="field name:"
                label-for="field_name"
            >
              <b-form-input
                  ref="field_name"
                  id="field_name"
                  v-model="field.name"
                  type="text"
                  placeholder="Enter field name"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Field name can not be empty!
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
      fields: [],
      isLoadingField: false,
      field: {},
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
    getField(id){
      ApiConnect.get('/fields/'+id).then((response) =>{
        this.field = response.data
      });
    },
    getBooks(){
      let params = {params: {"fields": this.field.name}};
      ApiConnect.get('/books/',params).then((response) =>{
        this.selectedBooks = response.data
      })
    },
    submit(){
      if (! this.field.name){
        this.$refs['field_name'].state = false;
        this.$refs['field_name'].value = "";
        return;
      }
        ApiConnect.put('/fields', this.field).then((response) =>{
          this.$refs["field_name"].state = null;
          this.makeToast('field  '+this.field.name+' has been updated successfully.')
        }).catch(error => {
          console.log(error)
        })
    },
    create(){
      if (! this.field.name){
        this.$refs['field_name'].state = false;
        this.$refs['field_name'].value = "";
        return;
      }
        ApiConnect.post('/fields', this.field).then((response) =>{
          this.makeToast('field '+this.field.name+' has been created successfully.')
          console.log(response)
        }).catch(error => {
          console.log(error)
        })

      ApiConnect.get('/fields/').then(resp =>{
        this.$router.push('/edit_fields/'+(resp.data[resp.data.length -1].id+1))
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
      this.field = {
            id: 0,
            name: '',
      }
    }else{
      this.getField(this.$route.params.id);
    }
    this.getBooks();
  },
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