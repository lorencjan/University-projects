<template>
      <b-container id="mynavbar">
        <b-row no-gutters>
          <b-col  cols="11" >
            <b-form-input style="width: 100%"
                          @input="sendToNavbar"
                          v-model="search_input"
                          placeholder="Search"
                          type="search"
            >

            </b-form-input>
          </b-col>
          <b-col  cols="1"><b-button style="width: 100%"
                                     variant="primary"
                                     type="submit">Search</b-button></b-col>
        </b-row>
      </b-container>
</template>

<script>
export default {
  name: 'SearchBar',
  data() {
    return {
    search_input: ''
    }
  },
  methods :{
    sendToNavbar(){
      this.$emit('event', this.search_input);
    }
  }
}
</script>
