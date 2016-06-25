#!/bin/bash

cd enttec
tar xfvz libftd2xx-arm-v7-hf-1.3.6.tgz
cd release/build
sudo cp libftd2xx.* /usr/local/lib
sudo chmod 0755 /usr/local/lib/libftd2xx.so.1.3.6
sudo ln -sf /usr/local/lib/libftd2xx.so.1.3.6 /usr/local/lib/libftd2xx.so

cd ..
cd examples
make -B