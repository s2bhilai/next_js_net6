import { Button } from 'flowbite-react';
import ButtonGroup from 'flowbite-react/lib/esm/components/Button/ButtonGroup';
import React from 'react'

type Props = {
    pageSize: number;
    setPageSize: (size: number) => void;
}

const pageSizeButtons = [4,8,12];

export default function Filters({pageSize,setPageSize}: Props) {
  return (
    <div className='flex justify-between items-center mb-4'>
        <div>
            <span className='uppercase text-sm text-gray-500 mr-2'>Page Size</span>
            <ButtonGroup>
                {pageSizeButtons.map((v,index) => {
                    return (
                        <Button key={index} onClick={() => setPageSize(v)}
                          color={ `${pageSize === v ? 'red' : 'gray'}` }
                          className='focus:ring-0'>
                            {v}
                        </Button>
                    );
                })}
            </ButtonGroup>
        </div>
    </div>
  )
}
