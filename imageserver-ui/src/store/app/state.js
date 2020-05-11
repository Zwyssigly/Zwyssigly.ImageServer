export default function () {
  return {
    galleries: [],
    galleries_loading: false,
    defaultConfiguration: {
      avoidDuplicates: true,
      sizes: [
        {
          tag: 'original',
          maxWidth: 2048,
          maxHeight: 2048,
          quality: 0.75,
          format: 'jpg'
        }
      ]
    },
    defaultSize: {
      quality: 0.75,
      format: 'jpg'
    },
    defaultAccount: {    
      type: 'Basic', 
      name: 'New-user',
      permissions: ['thumbnail:read'],
      password: null
    }
  }
}
