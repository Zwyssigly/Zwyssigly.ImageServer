<template>
  <q-input
    :type="visible ? 'text' : 'password'"
    :value="value"
    @input="val => $emit('input', val)"
    :disable="disable"
    :label="label"
  >
    <template v-slot:append>
      <q-icon
        :name="visible ? 'visibility' : 'visibility_off'"
        class="cursor-pointer"
        @click="visible = !visible"
      />
      <q-icon        
        name="shuffle"
        class="cursor-pointer"
        @click="random()"
      />
    </template>
  </q-input>
</template>

<script>
export default {
  name: 'PasswordInput',
  props: {
    value: String,
    label: {
      type: String,
      default: 'Password'
    },
    disable: Boolean
  },
  data () {
    return {
      visible: false,
      validChars: 'qwertzuiopasdfghjklyxcvbnmQWERTZUIOPASDFGHJKLYXCVBNM1234567890+-'
    };
  },
  methods: {
    random () {
      let password = [];
      for (let i = 0; i < 20; i++) 
        password.push(this.validChars.charAt(Math.floor(Math.random() * this.validChars.length)));
      this.$emit('input', password.join(''));
    }
  }
}
</script>