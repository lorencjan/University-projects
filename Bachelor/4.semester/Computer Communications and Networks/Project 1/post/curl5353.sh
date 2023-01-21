#!/bin/bash
# GET - ok or not found
curl -i localhost:5353/resolve?name=www.fit.vutbr.cz\&type=A
curl -i localhost:5353/resolve?name=apple.com\&type=A
curl -i localhost:5353/resolve?name=www.seznam.cz\&type=A
curl -i localhost:5353/resolve?name=147.229.9.23\&type=PTR
curl -i localhost:5353/resolve?name=17.142.160.59\&type=PTR
curl -i localhost:5353/resolve?name=77.75.75.176\&type=PTR
curl -i localhost:5353/resolve?name=www.sna2457839654.cz\&type=A
curl -i localhost:5353/resolve?name=1.1.1.8\&type=PTR
curl -i localhost:5353/resolve?name=a184-51-10-83.deploy.static.akamaitechnologies.com\&type=A
curl -i localhost:5353/resolve?name=this.is.verylong.domain.that.doesnotexists.in.the.internet.cz\&type=A
# GET  - invalid query
curl -i localhost:5353/preloz?name=www.fit.vutbr.cz\&type=A
curl -i localhost:5353/resolve?name=www.fit.vutbr.cz\&type=BAD
curl -i localhost:5353/resolve?name=www.fit.vutbr.cz
curl -i localhost:5353/resolve?nam=www.fit.vutbr.cz\&typ=A
curl -i localhost:5353/resolve?name=www.fit.vutbr.cz\&type=A&extra=lewrhofiuooweyrotfuywoeuygtifyuweoyurtfgowuyegroyfugwietyugofywueofuyhwoeuyrbfouyweroyufgwoeutyfgoweuygfhouywehobrfuywoeuytgowuyetor
# POST - ok expect post4, post 3 has also single input
curl -i --data-binary @post1 -X POST http://localhost:5353/dns-query
curl -i --data-binary @post2 -X POST http://localhost:5353/dns-query
curl -i --data-binary @post3 -X POST http://localhost:5353/dns-query
curl -i --data-binary @post4 -X POST http://localhost:5353/dns-query
curl -i --data-binary @post5 -X POST http://localhost:5353/dns-query
