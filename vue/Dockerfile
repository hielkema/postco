# base image
FROM node:15.0.1
 
# set working directory
RUN mkdir /usr/src/app
RUN mkdir /usr/src/app/node_modules
WORKDIR /usr/src/app
 
# add `/usr/src/app/node_modules/.bin` to $PATH
ENV PATH /usr/src/app/node_modules/.bin:$PATH
 
# install and cache app dependencies
COPY package*.json ./
RUN npm install --no-optional
# RUN npm install  @vue/cli
# RUN npm install vuetify sass sass-loader fibers deepmerge -D

# start app
CMD ["npm", "run", "serve"]