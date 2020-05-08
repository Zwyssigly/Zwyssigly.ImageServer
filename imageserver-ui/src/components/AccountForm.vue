<template>
  <div>
    <q-select
      label="Type"
      :value="value.type"
      :options="['Anonymous', 'Basic']"
      :rules="[val => val.length > 0 ? null : 'Type expected']"
      @input="val => updateType(val)"
    />
    <q-input 
      label="Name"
      :value="value.name"
      :rules="[val => val.length > 0 ? null : 'Name expected']"
      @input="val => emitValue({ name: val })"
      :disable="value.type==='Anonymous'"
    />
    <q-input 
      label="Password"
      :value="value.password"
      @input="val => emitValue({ password: val })"
      :disable="value.type==='Anonymous'"
      type="password"
    />
    <div class="text-label">Permissions: </div>
    <q-option-group
      :value="value.permissions"
      @input="val => emitValue({ permissions: val })"
      :options="permissions"
      color="primary"
      type="checkbox"
    />
  </div>
</template>

<script>
export default {
  name: 'AccountForm',
  props: {
    value: {
      type: Object,
      default: () => ({
        type: 'Basic',
        name: null,
        permissions: [],
        password: null,
      })
    }
  },
  data () {
    return {
      permissions: [
        { label: "thumbnail:read", value: 'thumbnail:read' },
        { label: "image:read", value: 'image:read' },
        { label: "image:write", value: 'image:write' },
        { label: "configuration:read", value: 'configuration:read' },
        { label: "configuration:write", value: 'configuration:write' },
        { label: "security", value: 'security' },
      ]
    }
  },
  methods: {
    updateType(val) {
      switch (val)
      {
        case 'Basic': 
          this.emitValue({ type: 'Basic' });
          break;
        case 'Anonymous':
          this.emitValue({ type: 'Anonymous', name: 'Anonymous', password: null });
          break;
      }
      
    },
    emitValue(val) {
      console.log(val);
      this.$emit('input', { ...this.$props.value, ...val });
    }
  }
}
</script>