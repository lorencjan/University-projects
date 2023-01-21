<template>
  <b-container class="author-form-list edit-book-page">
    <b-row class="text-center">
      <b-col>
        <font-awesome-icon icon="fa-solid fa-display" size="4x" v-b-modal.modal-preview type="button" class="preview"/>
      </b-col>
    </b-row>
    <b-row class="text-center mb-4">
      <b-col style="color: black">
        preview author page
      </b-col>
    </b-row>
    <b-form @submit.prevent="submit">
      <b-row class="form-row">
        <b-col>
        <b-form-group
            id="name-label"
            label="Name:"
            label-class="required"
            label-for="name"
        >
          <b-form-input
              ref="name"
              id="name"
              v-model="form.name"
              type="text"
              placeholder="Enter first name"
              required
          ></b-form-input>
          <b-form-invalid-feedback>
            Name field can not be empty!
          </b-form-invalid-feedback>
        </b-form-group>
        </b-col>
        <b-col>
          <b-form-group
              id="surname-label"
              label="Last name:"
              label-class="required"
              label-for="surname"
          >
            <b-form-input
                ref="surname"
                id="surname"
                v-model="form.surname"
                type="text"
                placeholder="Enter last name"
                required
            ></b-form-input>
            <b-form-invalid-feedback>
              Last name field can not be empty!
            </b-form-invalid-feedback>
          </b-form-group>
        </b-col>
      </b-row>

      <b-row class="form-row">
        <b-col>
          <b-form-group
          id="dateOfBirth-label"
          label="Date of birth:"
          label-for="dateOfBirth">
          <date-picker
              id="dateOfBirth"
              ref="dateOfBirth"
              v-model="form.dateOfBirth "
              type="date"
              required
              placeholder="Enter date of birth">
          </date-picker>
          </b-form-group>
        </b-col>
        <b-col>
          <b-form-group
              id="dateOfDeath-label"
              label="Date of death:"
              label-for="dateOfDeath">
          <date-picker
              id="dateOfDeath"
              ref="dateOfDeath"
              reset-button
              label-reset-button="Clear"
              v-model="form.dateOfDeath "
              type="date"
              placeholder="Enter date of death">
          </date-picker>
          </b-form-group>

        </b-col>
      </b-row>

      <b-row class="form-row">
        <b-col>
          <b-form-group
              id="photograph-label"
              label="Photograph:"
              label-for="photograph">
          <b-form-file
              v-model="photograph"
              id="photograph"
              @input="photographInputChange"
              ref="photograph"
              placeholder="Choose a file or drop it here..."
              drop-placeholder="Drop file here..."
          ></b-form-file>
          </b-form-group>
        </b-col>
        <b-col></b-col>
      </b-row>

      <b-row class="form-row">
        <b-col>
        <b-form-group
            id="description-label"
            label="Description:"
            label-for="description"
        >
          <b-form-textarea
              ref="description"
              id="description"
              v-model="form.description"
              placeholder="Enter description about author..."
              rows="8"

          ></b-form-textarea>
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

    <b-modal id="modal-preview" title="Preview" size="xl" hide-footer>
        <b-container>
            <AuthorBigTile
                :name="form.name"
                :surname="form.surname"
                :birth="new Date(form.dateOfBirth)"
                :death="new Date(form.dateOfDeath)"
                :description="form.description"
                :img = "form.photographPath"
            ></AuthorBigTile>

        </b-container>

    </b-modal>

  </b-container>
</template>

<script>
import ApiConnect from "@/services/ApiConnect";
import AuthorBigTile from "@/components/author/AuthorBigTile";
import * as file from "@/assets/js/file";
import DatePicker from "vue2-datepicker";

export default {
  name: 'AuthorForm',
  components: {
    AuthorBigTile,
    DatePicker
  },
  data() {
    return {
      show: true,
      photograph: null
    };
  },
  props: {
    author: {},
    form: {},
  },
  methods: {
    photographInputChange() {
      if (this.photograph !== null){
        this.photograph = file.renameFile(this.photograph);
      }
      let formData = new FormData();
      formData.append('file', this.photograph, this.photograph.name);
      ApiConnect.post('uploadFile', formData).then((response)=> {
        let filePath = response.data.fileDownloadUri;
        this.form.photographPath = filePath;
      })
    },
    check_author_form(){
      let form_check_error = false;
      if (! this.form.name){
        this.$refs['name'].state = false;
        this.$refs['name'].value = "";
        form_check_error = true;
      }
      if (! this.form.surname){
        this.$refs['surname'].state = false;
        this.$refs['surname'].value = "";
        form_check_error = true;
      }

      return form_check_error;
    },
    submit(){
      this.$refs['name'].state= null;
      this.$refs['surname'].state= null;
      if (this.check_author_form()) return;
      ApiConnect.put('/authors', this.form).then((response) =>{
        this.$refs['name'].state= null;
        this.$refs['surname'].state= null;
        this.makeToast('Author '+this.author.name+' ' + this.author.surname  +'has been updated successfully.')
      }).catch(error => {
        console.log(error)
      })
    },
    create(){
      this.$refs['name'].state= null;
      this.$refs['surname'].state= null;
      if (this.check_author_form()) return;
      ApiConnect.post('/authors', this.form).then((response) =>{
        this.makeToast('Author '+this.form.name+' ' + this.form.surname  +'has been created successfully.')
      }).catch(error => {
      })
      ApiConnect.get('/authors/').then(resp =>{
        this.$router.push('/edit_authors/'+(resp.data[resp.data.length -1].id+1))
      })
    },
    makeToast(text) {
      this.$bvToast.toast(text, {
        title: 'Library',
        variant: 'success',
        autoHideDelay: 5000,
      })
    },
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
