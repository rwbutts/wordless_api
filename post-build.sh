echo POST BUILD
if [ -d wwwroot ]; then
     rm -rf wwwroot
fi

mkdir wwwroot
echo cp -r ../../../wordless.vue/dist/. -t ./wwwroot 
cp -r ../../../wordless.vue/dist/* -t ./wwwroot 