(self.webpackChunk_N_E=self.webpackChunk_N_E||[]).push([[323],{813:(e,a,r)=>{Promise.resolve().then(r.bind(r,5714))},1872:(e,a,r)=>{"use strict";r.d(a,{T:()=>o});let o=e=>{window.chrome&&window.chrome.webview&&window.chrome.webview.postMessage(e)}},5714:(e,a,r)=>{"use strict";r.d(a,{default:()=>l});var o=r(5155),n=r(2115),i=r(1872);let s=e=>{let{lableName:a,defaultValue:r,webViewProps:s}=e,[c,l]=(0,n.useState)(r),d=e=>{let a=e.target.checked;l(a),s&&((0,i.T)({id:s.id,command:s.command,payload:{Value:a}}),console.log({Value:a}))};return(0,o.jsxs)(o.Fragment,{children:[(0,o.jsx)("input",{type:"checkbox",id:a,className:"cursor-pointer",onChange:e=>{d(e)},checked:c}),(0,o.jsx)("label",{htmlFor:a,className:"cursor-pointer",children:a})]})},c=e=>{let{webViewProps:a,name:r}=e;return(0,o.jsx)(o.Fragment,{children:(0,o.jsx)("button",{className:"cursor-pointer",onClick:()=>{a&&((0,i.T)({id:a.id,command:a.command,payload:void 0}),console.log("clicing"))},children:r})})},l=()=>(0,o.jsx)(o.Fragment,{children:(0,o.jsxs)("div",{className:"w-50 h-30 bg-amber-50 flex flex-col items-center justify-center select-none",children:[(0,o.jsx)("div",{className:"bg-amber-300 px-2 py-1 rounded-md cursor-pointer hover:bg-amber-500 active:opacity-50 transition-all duration-200",children:(0,o.jsx)(c,{webViewProps:{id:"flooding",command:"SELECT_TERRAIN"},name:"Select Terrain"})}),(0,o.jsx)("div",{className:"m-2 flex gap-2",children:(0,o.jsx)(s,{webViewProps:{id:"flooding",command:"RUN_SIMULATION"},lableName:"Run",defaultValue:!0})}),(0,o.jsx)("div",{className:"bg-amber-300 px-2 py-1 rounded-md cursor-pointer hover:bg-amber-500 active:opacity-50 transition-all duration-200",children:(0,o.jsx)(c,{webViewProps:{id:"flooding",command:"RESET_SIMULATION"},name:"Reset"})})]})})}},e=>{var a=a=>e(e.s=a);e.O(0,[441,684,358],()=>a(813)),_N_E=e.O()}]);